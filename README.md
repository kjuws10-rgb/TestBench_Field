# TestBench_Field / StageWin

StageWin은 Stage 장비를 중심으로 Scan 장비, Vision 장비, ACS Motion Controller, WAGO IO, LDS 센서를 연동하는 Windows Forms 기반 설비 제어 프로그램입니다.  
주 실행 프로젝트는 `StageWin/WinForms.csproj`이며, 솔루션 파일은 `StageWin.sln`입니다.

## 개발 환경

- IDE: Visual Studio 2022
- Framework: .NET Framework 4.8
- UI: Windows Forms
- 주요 외부 의존성
  - ACS Motion Control `ACS.SPiiPlusNET.dll`
  - NuGet `System.ValueTuple 4.5.0`
  - 카메라/모션 연동 라이브러리 `StageWin/CameraZLib/*.dll`, `*.lib`

ACS 라이브러리는 현재 프로젝트 파일에서 아래 로컬 설치 경로를 참조합니다.

```text
C:\Program Files (x86)\ACS Motion Control\SPiiPlus ADK Suite v2.60\SPiiPlus .NET Library\.NET Library Demo\x64\Release\ACS.SPiiPlusNET.dll
```

해당 장비 SDK가 설치되지 않은 PC에서는 빌드 전 참조 경로를 맞춰야 합니다.

## 프로젝트 구조

```text
StageWin.sln
├─ StageWin/   WinForms 실행 프로젝트, 메인 UI, 시퀀스, 장비 연동 허브
├─ Core/       설정, 레시피 모델/저장소, 로깅
├─ Network/    Stage-Scan-Vision TCP/RPC 프로토콜과 패킷 정의
├─ Motion/     ACS 모션 컨트롤러 어댑터와 안전 래퍼
├─ WagoIO/     WAGO Modbus TCP IO, IO 카탈로그, 가상 IO 버스
├─ Etc/        Safety 정책, 인터락 라우팅
├─ Lds/        Optex LDS 이더넷 드라이버와 폴링 허브
└─ Process/    공정 액션 인터페이스 보조 프로젝트
```

현재 `StageWin.sln`에는 `WinForms`, `Core`, `Network`, `LDS`, `Motion`, `WagoIO`, `Safety` 프로젝트가 포함되어 있습니다. `Process` 프로젝트는 폴더에는 존재하지만 현재 솔루션에는 포함되어 있지 않습니다.

## 실행 흐름

1. `StageWin/Program.cs`
   - `AppConfig.Load()`로 설정을 로드합니다.
   - Visual Styles를 활성화한 뒤 `Form1`을 실행합니다.

2. `Core/Config/AppConfig.cs`
   - 기본 설정 파일 위치는 `D:\AppConfig\appsettings.json`입니다.
   - 파일이 없으면 기본값으로 생성합니다.
   - 레시피, 좌표, 알람 로그, Tool/Power Recipe 경로를 보정하고 필요한 폴더를 생성합니다.

3. `StageWin/Form1.cs`
   - 프로그램의 중심 허브 역할을 합니다.
   - TCP 서버를 시작하고 Scan/Vision 클라이언트 세션을 관리합니다.
   - Motion, LDS, WAGO IO, Safety, Recipe UI, Auto/Semi Auto 시퀀스를 연결합니다.
   - 내부 탭에 Recipe, Optic, IO, Motion, Overview 등 하위 폼을 임베드합니다.

## 통신 구조

`Network/NetCommon/Protocol.cs`에 공통 RPC 프레임이 정의되어 있습니다.

- Header: `nCode`, `nDataLen`, `nUniqueID`, `nErrorCode`
- Session: `RpcSession`
- Router: `PacketRouter`
- Role: `Scan`, `Vision`, `Unknown`

주요 명령 범위:

- `1000~1011`: Stage -> Scan
  - 레시피 목록/추가/삭제/선택
  - 레시피 데이터 변경
  - Scan Stop, Laser Control, Scan Status, Process Scanning
  - Power Meter Measurement
- `1021~1022`: Scan -> Stage
  - Stage 모터 이동 요청
  - Stage 현재 위치 요청
- `3000~3008`: Stage <-> Vision
  - Glass Align, 2nd Align
  - Mark Find
  - Flying Ready / Move Done
  - Vision 측 수동 검사 요청

`Form1.StartServer()`는 `AppConfig.StagePort` 기준으로 TCP 서버를 열고, 접속한 클라이언트의 `HELLO` 또는 고정 IP 매핑으로 Scan/Vision 역할을 구분합니다.

## 장비 연동

### Motion

`Motion/IMotionController.cs`가 공통 축 제어 인터페이스를 정의합니다.

- 지원 축: `Y = 0`, `X = 4`
- 주요 기능: Connect, Servo On/Off, Home, Stop, Abs/Rel Move, Jog, 위치/속도/가속도 조회
- `AcsMotionAdapter`는 ACS SPiiPlus .NET API를 감싸 실제 컨트롤러 또는 시뮬레이터에 연결합니다.
- `SafeMotionController`는 Servo On 등 주요 동작 전에 Safety 정책을 적용하는 래퍼입니다.

### WAGO IO

`WagoIO` 프로젝트는 Modbus TCP 기반 WAGO IO를 다룹니다.

- `modbusTCP.cs`: Modbus TCP Master 구현
- `WagoIoDriver.cs`: IO 설정 파일 파싱
- `IoCatalog.cs`, `VirtualVariables.cs`: IO 이름/상태 관리와 가상 버스

기본 설정은 `AppConfig`의 `WagoIp`, `WagoPort`, `WagoCfgPath`, `WagoUseSimulator` 값을 사용합니다.

### LDS

`Lds/OptexLdsAdapter.cs`는 Optex CDX 계열 LDS 센서를 TCP로 읽습니다.

- Binary 프로토콜 또는 ASCII Line 프로토콜 지원
- `LdsPollingHub`가 백그라운드 폴링을 수행합니다.
- 실측 시점에는 폴링을 잠시 멈추고 단발 측정을 수행할 수 있습니다.

### Safety

`Etc/SafetyPolicy.cs`와 `Etc/SafetyRouter.cs`가 인터락 정책을 담당합니다.

- Door Unlock 조건
- Laser On 상태에서 Door Unlock 차단
- Manual/Semi/Auto 모드별 출력 제어 제한
- ACS/Ajin MC 출력 조건
- Cooling Valve 제어 조건
- Auto Key 및 Motor Temp 체크

Safety는 UI 출력 제어와 Motion Servo On 경로에서 모두 사용됩니다.

## 레시피와 설정 파일

기본 설정 루트:

```text
D:\AppConfig
```

기본 데이터 경로:

```text
D:\ConfigPath\Recipes
D:\ConfigPath\Recipes.Scan
D:\ConfigPath\Coords
D:\AppLog\alarm_history.csv
D:\AppLog\ReviewMeasure
```

레시피 모델은 `Core/Recipe/RecipeStore.cs`에 정의되어 있습니다.

- Grid: Lines, HolesPerLine, PitchX, PitchY
- Offset: Scan/Review 기준 보정값
- Criteria: 측정 허용 오차
- Tooling: Power, Freq, ProcessSpeed, ShotCount, JumpSpeed, Attenuator
- Power Meter: 주파수별 목표 파워/Attenuator 테이블
- Review/Scan Flying: Flying 보정, Serpentine, 주행 방향, 속도, 시작 Row/Col

## Auto / Semi Auto 시퀀스

`StageWin/AutoProcess.cs`는 자동 시퀀스 ID를 정의합니다.

```text
Load
MoveToAlign
Align
PowerMeter
ProcessReady
Process
Inspection
MoveToAlignCheck
AlignCheck
MoveToUnload
Unload
```

실제 실행은 `Form1` 내부의 시퀀스 메서드들이 담당합니다. `DryRun` 모드에서는 장비 동작 또는 레이저 가공 없이 흐름 점검 용도로 사용할 수 있게 설계되어 있습니다.

## 빌드 방법

1. Visual Studio 2022에서 `StageWin.sln`을 엽니다.
2. NuGet 패키지를 복원합니다.
3. ACS SPiiPlus ADK가 설치되어 있는지 확인합니다.
4. 빌드 구성을 `Debug|x64` 또는 `Release|x64`로 선택합니다.
5. 시작 프로젝트를 `WinForms`로 설정하고 빌드/실행합니다.

명령줄 빌드 예시:

```powershell
msbuild StageWin.sln /p:Configuration=Debug /p:Platform=x64
```

### 빌드 검증 기록

현재 저장소 기준으로 `Debug|x64` 빌드를 시도했습니다.

- `WagoIO.csproj`에는 x64 출력 설정이 없어 빌드가 중단되었고, 다른 프로젝트와 같은 방식으로 `Debug|x64`, `Release|x64` PropertyGroup을 추가했습니다.
- 이후 `LDS`, `Network`, `WagoIO`, `Core`, `Safety` 프로젝트는 빌드 단계까지 진행되었습니다.
- 최종적으로 현재 PC에 `ACS.SPiiPlusNET.dll`이 설치되어 있지 않아 `Motion` 프로젝트에서 빌드가 실패했습니다.

ACS SDK 설치 또는 프로젝트 참조 경로 보정 후 다시 빌드해야 합니다.

## 운영 전 확인 사항

- `D:\AppConfig\appsettings.json`의 장비 IP/Port를 현장 네트워크에 맞게 조정해야 합니다.
- `NetMode`, `FixedRoleMap`, `StageServerIp`, `LoopbackScanIp`, `LoopbackVisionIp`를 Scan/Vision 연결 방식에 맞춰 설정해야 합니다.
- `WagoCfgPath`의 IO 설정 파일이 존재해야 Safety/IO 화면이 정상 동작합니다.
- ACS 실장비 없이 테스트할 경우 `AcsUseSimulator`를 `true`로 설정합니다.
- WAGO 실장비 없이 테스트할 경우 `WagoUseSimulator`를 `true`로 설정합니다.
- LDS 실장비 없이 테스트할 경우 `LdsUseSimulator`를 `true`로 설정합니다.

## 코드 분석 요약

- `Form1.cs`가 UI, 장비 연결, 통신 서버, 시퀀스, 레시피 이벤트를 대부분 직접 연결하고 있어 기능 중심은 명확하지만 파일 크기와 책임이 큽니다.
- 설정 경로가 `D:\AppConfig`, `D:\ConfigPath`, `D:\AppLog`처럼 절대 경로에 강하게 묶여 있습니다. 배포 PC 기준으로는 편하지만 개발/테스트 환경을 바꿀 때는 설정 주입이나 상대 경로 옵션이 필요할 수 있습니다.
- ACS 라이브러리 참조가 로컬 설치 경로에 고정되어 있어 신규 PC 세팅 시 빌드 실패 가능성이 있습니다.
- `Network` 계층은 Header + Body 기반의 단순 RPC 구조로 되어 있고, `nUniqueID`를 이용해 요청/응답을 매칭합니다. 세션별 전송 락을 둬 Header/Body 전송 순서를 보호합니다.
- `BinMarshal.FromBytes`는 수신 바디 길이가 다를 때도 제로 패딩/트림을 허용합니다. 장비 호환성에는 유리하지만 패킷 형식 오류를 조기에 잡기 어렵기 때문에 엄격 검증이 필요한 명령은 `FromBytesStrict`를 사용해야 합니다.
- Safety 정책은 IO 신선도(`UpdatedAt`)를 확인하고 미확정 상태를 조용히 차단하는 방향입니다. 초기 IO 동기화 중 오동작을 줄이는 의도가 보입니다.
- `Process` 프로젝트는 `Core`를 참조하지만 현재 솔루션에는 포함되어 있지 않아 실제 빌드/운영 경로에서 사용되는지 추가 확인이 필요합니다.
- 일부 Vision RPC timeout 값이 매우 크게 설정되어 있습니다. 현장 디버깅 목적일 수 있으나, 운영 모드에서는 장애 감지 지연으로 이어질 수 있습니다.

## Git 관리

이 저장소는 소스와 필요한 참조 라이브러리만 추적합니다. 아래 항목은 `.gitignore`로 제외합니다.

- `.vs/`
- `bin/`
- `obj/`
- `packages/`
- Visual Studio 사용자 설정 파일
- 로그/임시 파일

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using ModbusTCP;

namespace StageWin.WagoIO
{
    public static class CfgParser
    {
        // 새 포맷: Name  Address  SubAddress  Index  Status(false)  Status(true)
        // 아날로그(AO/AI)는 false/true 라벨이 없는 5열 형태.
        public static List<IOData> Parse(string filePath)
        {
            var list = new List<IOData>();
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return list;

            var lines = File.ReadAllLines(filePath);
            foreach (var raw in lines)
            {
                var line = raw.Trim();
                if (line.Length == 0) continue;
                if (line.StartsWith("#")) continue; // 주석/헤더 스킵

                // 공백 단위 토큰화 (Name은 공백 포함 → 뒤에서 역추적)
                var tokens = Regex.Split(line, @"\s+").Where(t => t.Length > 0).ToArray();
                if (tokens.Length < 5) continue; // 최소: Name.. + Address + SubAddress + Index

                // 뒤에서부터 컬럼을 끊는다
                // case-1) 디지털(6+ 토큰): ... | Address | SubAddress | Index | False | True
                // case-2) 아날로그(5 토큰):   ... | Address | SubAddress | Index
                string statusTrue = null, statusFalse = null;
                string indexStr, subAddr, addr;

                if (tokens.Length >= 6)
                {
                    statusTrue = tokens[tokens.Length - 1];
                    statusFalse = tokens[tokens.Length - 2];
                    indexStr = tokens[tokens.Length - 3];
                    subAddr = tokens[tokens.Length - 4];
                    addr = tokens[tokens.Length - 5];
                }
                else
                {
                    indexStr = tokens[tokens.Length - 1];
                    subAddr = tokens[tokens.Length - 2];
                    addr = tokens[tokens.Length - 3];
                }

                // 나머지는 Name
                var nameTokensCount = tokens.Length - (statusTrue != null ? 5 : 3);
                if (nameTokensCount <= 0) continue;
                string name = string.Join(" ", tokens, 0, nameTokensCount);

                int idx = 0; int.TryParse(indexStr, out idx);

                list.Add(new IOData
                {
                    Name = name,
                    StatusCode = addr,
                    SubAddressHex = subAddr,
                    Index = idx,
                    FalseLabel = statusFalse,
                    TrueLabel = statusTrue
                });
            }
            return list;
        }
    }

    
}
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    static class Diff
    {
        public static IEnumerable<Hunk> Parse(string diff)
        {
            IEnumerable<string> lines = ToLines(diff).Skip(4);
            return ToHunks(lines);
        }

        static IEnumerable<string> ToLines(string str)
        {
            List<string> res = new List<string>();
            using (StringReader r = new StringReader(str))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                    res.Add(line);
            }
            return res;
        }

        static IEnumerable<Hunk> ToHunks(IEnumerable<string> lines)
        {
            List<Hunk> hunks = new List<Hunk>();
            if (!lines.Any())
                return hunks;
            List<string> hunkLines = new List<string>() { lines.First() };
            lines.Skip(1).ToList().ForEach(l =>
            {
                if (l.First() == '@')
                {
                    hunks.Add(new Hunk(hunkLines));
                    hunkLines = new List<string>();
                }
                else
                    hunkLines.Add(l);
            });
            if (hunkLines.Any())
                hunks.Add(new Hunk(hunkLines));
            return hunks;
        }
    }
}
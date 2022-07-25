using System;
using System.Collections.Generic;

namespace wodat
{

        class ArgumentParser
        {
            public string Command { get; set; }
            public string DetailCommand { get; set; }
            public Dictionary<string, string> Parameters { get; set; }

            private List<string> original;

            public ArgumentParser(string[] arguments)
            {
                Parameters = new Dictionary<string, string>();
                if (arguments != null)
                {
                    original = new List<string>(arguments);
                    Parse();
                }
            }

            private void Parse()
            {
                if (original.Count > 0)
                {
                    this.Command = original[0];
                }
                if (original.Count > 1)
                {
                    if (!original[1].StartsWith("-"))
                    {
                        this.DetailCommand = original[1];
                    }
                    original.ForEach(i => {
                        if (i.StartsWith("-"))
                        {
                            int pos = i.IndexOf(':');
                            if (pos == -1)
                            {
                                this.Parameters.Add(i.Substring(1), null);
                            }
                            else
                            {
                                this.Parameters.Add(i.Substring(1, pos - 1), i.Substring(pos + 1));
                            }
                        }
                    });
                }
            }
        }
    }


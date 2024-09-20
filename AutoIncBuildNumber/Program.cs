using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace AutoIncBuildNumber
{
    enum VERSION_PART
    {
        MAJOR = 0,
        MINOR = 1,
        BUILD = 2,
        REVISION = 3
    }

    class Program
    {
        private static string PrettifyXML(XmlDocument doc)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Encoding = Encoding.UTF8,
                //CloseOutput = true,
                Indent = true,
                IndentChars = "  ",
                NewLineChars = Environment.NewLine,
                NewLineHandling = NewLineHandling.Replace
            };
            StringBuilder sb = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                doc.Save(writer);
            }
            //text = Regex.Replace(text, $"</{tag}>", "</$1>" + Environment.NewLine + "    ");
            //return sb.ToString().Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "").Trim();
            return sb.ToString();
        }

        /// <summary>
        /// Increment Revision Number
        /// </summary>
        /// <param name="strVersion"></param>
        /// <returns></returns>
        private static string incBuildNo(string strVersion)
        {
            string[] version = strVersion.Split('.');

            if (version.Length == 4)
            {
                try
                {
                    int buildNo = Convert.ToInt32(version[3]);
                    version[3] = Convert.ToString(buildNo + 1);
                    return (String.Join(".", version));
                }
                catch
                {
                    return (strVersion);
                }
            }
            return (strVersion);
        }

        /// <summary>
        /// Increment Specified version's number, major, minor, revision, build
        /// </summary>
        /// <param name="strVersion"></param>
        /// <param name="partnum"></param>
        /// <returns></returns>
        private static string incBuildNo(string strVersion, VERSION_PART part = VERSION_PART.REVISION)
        {
            string[] version = strVersion.Split('.');
            int version_idx = (int)part;

            if (version_idx >= version.Length) version_idx = version.Length - 1;
            if (version_idx < 0) version_idx = 0;

            try
            {
                if (version[version.Length - 1].Trim().Equals("*", StringComparison.CurrentCultureIgnoreCase))
                {
                    version[version.Length - 1] = "0";
                }
                int buildNo = Convert.ToInt32(version[version_idx]);
                version[version_idx] = Convert.ToString(buildNo + 1);
                if (version_idx == 0) { version[1] = "0"; version[2] = "0"; version[3] = "0"; }
                else if (version_idx == 1) { version[2] = "0"; version[3] = "0"; }
                else if (version_idx == 2) { version[3] = "0"; }
                Console.WriteLine($"Version From: [{strVersion}] To: [{string.Join(".", version)}]");
                return (string.Join(".", version));
            }
            catch
            {
                return (strVersion);
            }
        }

        private static string[] ParseCommandLine(string cmdline)
        {
            List<string> args = new List<string>();

            string[] cmds = cmdline.Split( new char[] { ' ' } );
            string arg = "";
            foreach (string cmd in cmds)
            {
                if (cmd.StartsWith("\"") && cmd.EndsWith("\""))
                {
                    args.Add(cmd.Trim(new char[] { '\"', ' ' }));
                    arg = "";
                }
                else if (cmd.StartsWith("\""))
                {
                    arg = cmd + " ";
                }
                else if (cmd.EndsWith("\""))
                {
                    arg += cmd;
                    args.Add(arg.Trim(new char[] { '\"', ' ' }));
                    arg = "";
                }
                else if (!string.IsNullOrEmpty(arg))
                {
                    arg += cmd + " ";
                }
                else
                {
                    if (!string.IsNullOrEmpty(cmd))
                    {
                        args.Add(cmd);
                    }
                    arg = "";
                }
#if DEBUG
                Console.WriteLine( $"Curent ARG: {cmd}, Parsed ARG: {arg}" );
#endif
            }
            return (args.GetRange(1, args.Count - 1).ToArray());
        }

        static void Main(string[] args)
        {
            string[] param = ParseCommandLine(Environment.CommandLine);
#if DEBUG
            Console.WriteLine( Environment.CommandLine );
            //Console.WriteLine( string.Join( " : ", Environment.GetCommandLineArgs() ) );
            Console.WriteLine( string.Join( " : ", param ) );
#endif
            string[] lineSeparators = new string[] {"\n\r", "\r\n", "\r", "\n"};

            if (param.Length < 1)
            {
                return;
            }

            string projectFolder = param[0].Trim(new char[] { '\"', ' ' } );
#if DEBUG
            Console.WriteLine( projectFolder );
#endif

            VERSION_PART versionPart = VERSION_PART.REVISION;
            if (param.Length >= 2)
            {
                try
                {
                    versionPart = (VERSION_PART)Enum.Parse(typeof(VERSION_PART), param[1].Trim(), true);
                    // if using .net framework > v4.0 can using following codes
                    //if (Enum.TryParse( param[1].Trim(), true, out versionPart ))
                    //{
#if DEBUG
                    //    Console.WriteLine( versionPart );
#endif
                    //}
                }
                catch { }
            }
#if DEBUG
            Console.WriteLine( projectFolder );
            Console.WriteLine( versionPart );
#endif
            List<string> assemblyInfoSource = new List<string>
            {
                $@"{projectFolder}\Properties\AssemblyInfo.cs",
                $@"{projectFolder}\AssemblyInfo.cs",
                $@"{projectFolder}\*.csproj"
            };

            string strAssemblyVersion = "[assembly: AssemblyVersion( \"";
            string strAssemblyFileVersion = "[assembly: AssemblyFileVersion( \"";
            string strVersionEnd = "\" )]";
            Regex pattenAssemblyVersion = new Regex( @"^\[assembly:( )*?Assembly(File)*?Version\(( )*?""(\d+)\.(\d+)\.(\d+)\.(\d+)""( )*?\)( )*?\].*?$" );

            bool changed = false;

            foreach (string assemblyInfoFile in assemblyInfoSource)
            {
                var ext = Path.GetExtension(assemblyInfoFile).ToLower();
#if DEBUG
                    Console.WriteLine( assemblyInfoFile );
#endif
                if (ext.Equals(".cs") && File.Exists(assemblyInfoFile))
                {
                    string assemblyInfo = File.ReadAllText(assemblyInfoFile, Encoding.UTF8);
                    if (assemblyInfo.Contains("[assembly: AssemblyVersion(") || assemblyInfo.Contains("[assembly: AssemblyFileVersion("))
                    {
                        string[] lines = assemblyInfo.Split(lineSeparators, StringSplitOptions.None);

                        for (int idx = 0; idx < lines.Length; idx++)
                        {
                            string line = lines[idx];
                            Match mo = pattenAssemblyVersion.Match(line);

                            if (line.StartsWith(strAssemblyVersion) && line.EndsWith(strVersionEnd))
                            {
                                string strVersion = line.Substring(strAssemblyVersion.Length, line.Length - strAssemblyVersion.Length - strVersionEnd.Length);
                                lines[idx] = line.Replace(strVersion, incBuildNo(strVersion, versionPart));
                                changed = true;
                            }
                            else if (line.StartsWith(strAssemblyFileVersion) && line.EndsWith(strVersionEnd))
                            {
                                string strVersion = line.Substring(strAssemblyFileVersion.Length, line.Length - strAssemblyFileVersion.Length - strVersionEnd.Length);
                                lines[idx] = line.Replace(strVersion, incBuildNo(strVersion, versionPart));
                                changed = true;
                            }
                            else if (mo.Length > 0)
                            {
                                string strVersion = $"{mo.Groups[4]}.{mo.Groups[5]}.{mo.Groups[6]}.{mo.Groups[7]}";
                                lines[idx] = line.Replace(strVersion, incBuildNo(strVersion, versionPart));
                                changed = true;
                            }
                        }
                        if (string.IsNullOrEmpty(lines[lines.Length - 1]))
                        {
                            lines = string.Join("\r", lines).Trim().Split(lineSeparators, StringSplitOptions.None);
                            changed = true;
                        }
                        if (changed)
                        {
                            File.WriteAllLines(assemblyInfoFile, lines, Encoding.UTF8);
                            break;
                        }
                    }
                }
                else if (ext.Equals(".csproj"))
                {
                    var csprojs = Directory.GetFiles(projectFolder, "*.csproj", SearchOption.TopDirectoryOnly);
                    if (csprojs.Length > 0)
                    {
                        var version_tags = new string[]{ "Version", "AssemblyVersion", "FileVersion" };
                        var csproj = csprojs[0];
                        var xml = new XmlDocument();
                        xml.PreserveWhitespace = true;
                        xml.Load(csproj);

                        var targets = xml.GetElementsByTagName("TargetFramework");
                        if (targets.Count > 0)
                        {
                            var root = targets[0].ParentNode;
                            foreach (var tag in version_tags)
                            {
                                var tags = xml.GetElementsByTagName(tag);
                                if (tags.Count > 0 && tags[0].ParentNode == root)
                                {
                                    var ver_tag = tags[0];
                                    if (ver_tag.Value != null)
                                    {
                                        ver_tag.Value = incBuildNo(ver_tag.Value, versionPart);
                                        changed = true;
                                    }
                                    else if (ver_tag.InnerText != null)
                                    {
                                        ver_tag.InnerText = incBuildNo(ver_tag.InnerText, versionPart);
                                        changed = true;
                                    }
                                }
                                else
                                {
                                    var ver_tag = xml.CreateNode(XmlNodeType.Element, tag, xml.NamespaceURI);
                                    ver_tag.InnerText = "1.0.0.0";
                                    root.AppendChild(ver_tag);
                                    root.AppendChild(xml.CreateTextNode(Environment.NewLine));
                                    changed = true;
                                }
                            }
                        }
                        if (changed)
                        {
                            var lines = PrettifyXML(xml).Split(lineSeparators, StringSplitOptions.None);
                            for (var i = 0; i < lines.Length; i++)
                            {
                                var line = lines[i];
                                foreach (var tag in version_tags)
                                {
                                    if (Regex.IsMatch(line, $"<{tag}>(.+)</{tag}>", RegexOptions.IgnoreCase))
                                    {
                                        lines[i] = "    " + line.Trim();
                                    }
                                }
                                if (Regex.IsMatch(line, @"</?PropertyGroup>", RegexOptions.IgnoreCase))
                                {
                                    lines[i] = "  " + line.Trim();
                                }
                            }
                            File.WriteAllText(csproj, string.Join(Environment.NewLine, lines), Encoding.UTF8);
                            break;
                        }
                    }
                }
            }
        }
    }
}

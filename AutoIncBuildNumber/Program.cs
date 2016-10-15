using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

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
        /// <summary>
        /// Increment Revision Number
        /// </summary>
        /// <param name="strVersion"></param>
        /// <returns></returns>
        private static string incBuildNo( string strVersion )
        {
            string[] version = strVersion.Split('.');

            if ( version.Length == 4 )
            {
                try
                {
                    int buildNo = Convert.ToInt32(version[3]);
                    version[3] = Convert.ToString( buildNo + 1 );
                    return ( String.Join( ".", version ) );
                }
                catch
                {
                    return ( strVersion );
                }
            }
            return ( strVersion );
        }

        /// <summary>
        /// Increment Specified version's number, major, minor, revision, build
        /// </summary>
        /// <param name="strVersion"></param>
        /// <param name="partnum"></param>
        /// <returns></returns>
        private static string incBuildNo(string strVersion, VERSION_PART part = VERSION_PART.REVISION )
        {
            string[] version = strVersion.Split('.');
            int version_idx = (int)part;

            if ( version_idx >= version.Length ) version_idx = version.Length - 1;
            if ( version_idx < 0 ) version_idx = 0;

            try
            {
                if (version[version.Length-1].Trim().Equals("*", StringComparison.CurrentCultureIgnoreCase ))
                {
                    version[version.Length - 1] = "0";
                }
                int buildNo = Convert.ToInt32(version[version_idx]);
                version[version_idx] = Convert.ToString( buildNo + 1 );
                Console.WriteLine( $"Version From: [{ strVersion }] To: [{string.Join( ".", version)}]" );
                return ( string.Join( ".", version ) );
            }
            catch
            {
                return ( strVersion );
            }
        }

        private static string[] ParseCommandLine(string cmdline)
        {
            List<string> args = new List<string>();

            string[] cmds = cmdline.Split( new char[] { ' ' } );
            string arg = "";
            foreach ( string cmd in cmds )
            {
                if ( cmd.StartsWith( "\"" ) && cmd.EndsWith( "\"" ) )
                {
                    args.Add( cmd.Trim( new char[] { '\"', ' ' } ) );
                    arg = "";
                }
                else if( cmd.StartsWith( "\"" ) )
                {
                    arg = cmd + " ";
                }
                else if ( cmd.EndsWith( "\"" ) )
                {
                    arg += cmd;
                    args.Add( arg.Trim( new char[] { '\"', ' ' } ) );
                    arg = "";
                }
                else if (!string.IsNullOrEmpty(arg))
                {
                    arg += cmd + " ";
                }
                else
                {
                    if ( !string.IsNullOrEmpty( cmd ) )
                    {
                        args.Add( cmd );
                    }
                    arg = "";
                }
#if DEBUG
                Console.WriteLine( $"Curent ARG: {cmd}, Parsed ARG: {arg}" );
#endif
            }            
            return ( args.GetRange( 1, args.Count - 1 ).ToArray() );
        }

        static void Main( string[] args )
        {
            string[] param = ParseCommandLine(Environment.CommandLine);
#if DEBUG
            Console.WriteLine( Environment.CommandLine );
            //Console.WriteLine( string.Join( " : ", Environment.GetCommandLineArgs() ) );
            Console.WriteLine( string.Join( " : ", param ) );
#endif
            string strAssemblyVersion = "[assembly: AssemblyVersion( \"";
            string strAssemblyFileVersion = "[assembly: AssemblyFileVersion( \"";
            string strVersionEnd = "\" )]";
            Regex pattenAssemblyVersion = new Regex( @"^\[assembly:( )*?Assembly(File)*?Version\(( )*?""(\d+)\.(\d+)\.(\d+)\.(\d+)""( )*?\)( )*?\].*?$" );

            string[] lineSeparators = new string[] {"\n\r", "\r\n", "\r", "\n"};

            if( param.Length < 1)
            {
                return;
            }

            string projectFolder = param[0].Trim(new char[] { '\"', ' ' } );
#if DEBUG
            Console.WriteLine( projectFolder );
#endif

            VERSION_PART versionPart = VERSION_PART.REVISION;
            if ( param.Length >= 2 )
            {
                try
                {
                    versionPart = (VERSION_PART) Enum.Parse( typeof( VERSION_PART ), param[1].Trim(), true );
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
            List<string> assemblyInfoSource = new List<string>();
            assemblyInfoSource.Add( $@"{projectFolder}\Properties\AssemblyInfo.cs" );
            assemblyInfoSource.Add( $@"{projectFolder}\AssemblyInfo.cs" );

            foreach ( string assemblyInfoFile in assemblyInfoSource )
            {
                if ( File.Exists( assemblyInfoFile ) )
                {
#if DEBUG
                    Console.WriteLine( assemblyInfoFile );
#endif

                    string assemblyInfo = File.ReadAllText( assemblyInfoFile, Encoding.UTF8 );
                    string[] lines = assemblyInfo.Split(lineSeparators, StringSplitOptions.None);

                    for ( int idx = 0; idx < lines.Length; idx++ )
                    {
                        string line = lines[idx];
                        Match mo = pattenAssemblyVersion.Match(line);

                        if ( line.StartsWith( strAssemblyVersion ) &&
                            line.EndsWith( strVersionEnd ) )
                        {
                            string strVersion = line.Substring( strAssemblyVersion.Length, line.Length - strAssemblyVersion.Length - strVersionEnd.Length );
                            lines[idx] = line.Replace( strVersion, incBuildNo( strVersion, versionPart ) );
                        }
                        else if ( line.StartsWith( strAssemblyFileVersion ) &&
                            line.EndsWith( strVersionEnd ) )
                        {
                            string strVersion = line.Substring( strAssemblyFileVersion.Length, line.Length - strAssemblyFileVersion.Length - strVersionEnd.Length );
                            lines[idx] = line.Replace( strVersion, incBuildNo( strVersion, versionPart ) );
                        }
                        else if( mo.Length >0)
                        {
                            string strVersion = $"{mo.Groups[4]}.{mo.Groups[5]}.{mo.Groups[6]}.{mo.Groups[7]}";
                            lines[idx] = line.Replace( strVersion, incBuildNo( strVersion, versionPart ) );
                        }

                    }
                    if ( string.IsNullOrEmpty( lines[lines.Length - 1] ) )
                    {
                        lines = string.Join( "\r", lines ).Trim().Split( lineSeparators, StringSplitOptions.None );
                    }
                    File.WriteAllLines( assemblyInfoFile, lines, Encoding.UTF8 );
                }
            }
        }
    }
}

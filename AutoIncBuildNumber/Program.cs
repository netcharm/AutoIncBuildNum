using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AutoIncBuildNumber
{
    class Program
    {
        private static string incBuildNo(string strVersion)
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

        static void Main( string[] args )
        {
            string strAssemblyVersion = "[assembly: AssemblyVersion( \"";
            string strAssemblyFileVersion = "[assembly: AssemblyFileVersion( \"";
            string strVersionEnd = "\" )]";

            string[] lineSeparators = new string[] {"\n\r", "\r\n", "\r", "\n"};

            string projectFolder = args[0].Trim(new char[] { '\"', ' '} );

            List<string> assemblyInfoSource = new List<string>();
            assemblyInfoSource.Add( $@"{projectFolder}\Properties\AssemblyInfo.cs" );
            assemblyInfoSource.Add( $@"{projectFolder}\AssemblyInfo.cs" );

            foreach ( string assemblyInfoFile in assemblyInfoSource )
            {
                if ( File.Exists( assemblyInfoFile ) )
                {
                    Console.WriteLine( assemblyInfoFile );

                    string assemblyInfo = File.ReadAllText( assemblyInfoFile, Encoding.UTF8 );
                    string[] lines = assemblyInfo.Split(lineSeparators, StringSplitOptions.None);

                    for ( int idx = 0; idx < lines.Length; idx++ )
                    {
                        string line = lines[idx];
                        if ( line.StartsWith( strAssemblyVersion ) &&
                            line.EndsWith( strVersionEnd ) )
                        {
                            string strVersion = line.Substring( strAssemblyVersion.Length, line.Length - strAssemblyVersion.Length - strVersionEnd.Length );
                            lines[idx] = line.Replace( strVersion, incBuildNo( strVersion ) );
                        }
                        else if ( line.StartsWith( strAssemblyFileVersion ) &&
                            line.EndsWith( strVersionEnd ) )
                        {
                            string strVersion = line.Substring( strAssemblyFileVersion.Length, line.Length - strAssemblyFileVersion.Length - strVersionEnd.Length );
                            lines[idx] = line.Replace( strVersion, incBuildNo( strVersion ) );
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

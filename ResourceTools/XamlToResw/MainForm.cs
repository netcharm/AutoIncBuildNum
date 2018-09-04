using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XamlToResw
{
    public partial class MainForm : Form
    {
        private string[] ParseCommandLine(string cmdline)
        {
            List<string> args = new List<string>();

            string[] cmds = cmdline.Split(new char[] { ' ' });
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
                Console.WriteLine($"Curent ARG: {cmd}, Parsed ARG: {arg}");
#endif
            }
            return (args.GetRange(1, args.Count - 1).ToArray());
        }

        private Dictionary<string, string> ParseXamlString(string[] lines)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            //System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
            //xml.LoadXml(string.Join("\n", lines));

            using (var ms = new MemoryStream())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(string.Join("", lines));
                ms.Write(buffer, 0, buffer.Length);
                ms.Seek(0, SeekOrigin.Begin);

                XDocument xmlx = XDocument.Load(ms);
                XElement page = xmlx.Root;
                //var all = xmlx.Root.DescendantNodes().OfType<XElement>();//.Select(x => x.Name).Distinct();

                if (page != null)
                {
                    var childs = page.Elements();
                    try
                    {
                        IEnumerable<XElement> elements = page.XPathSelectElements("//*");
                        foreach (var element in elements)
                        {
                            if (element.HasAttributes)
                            {
                                var uid = string.Empty;
                                var name = string.Empty;
                                var header = string.Empty;
                                var titlt = string.Empty;
                                var label = string.Empty;
                                var tooltip = string.Empty;
                                var text = string.Empty;
                                var placeholder = string.Empty;
                                var content = string.Empty;
                                var panetitle = string.Empty;

                                #region Detect UI string
                                Dictionary<string, string> attrs = new Dictionary<string, string>();
                                foreach (var attr in element.Attributes())
                                {
                                    if (attr.Name.LocalName.Equals("Name", StringComparison.CurrentCultureIgnoreCase) &&
                                       attr.Name.Namespace.NamespaceName.Equals("http://schemas.microsoft.com/winfx/2006/xaml", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        name = attr.Value;
                                        if (string.IsNullOrEmpty(uid)) uid = attr.Value;
                                    }
                                    else if (attr.Name.LocalName.Equals("Uid", StringComparison.CurrentCultureIgnoreCase) &&
                                       attr.Name.Namespace.NamespaceName.Equals("http://schemas.microsoft.com/winfx/2006/xaml", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        uid = attr.Value;
                                    }
                                    else if (attr.Name.LocalName.Equals("Header", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        attrs.Add($"{uid}.Header", attr.Value);
                                    }
                                    else if (attr.Name.LocalName.Equals("Title", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        attrs.Add($"{uid}.Title", attr.Value);
                                    }
                                    else if (attr.Name.LocalName.Equals("Label", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        attrs.Add($"{uid}.Label", attr.Value);
                                    }
                                    else if (attr.Name.LocalName.Equals("ToolTipService.ToolTip", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        attrs.Add($"{uid}.ToolTipService.ToolTip", attr.Value);
                                    }
                                    else if (attr.Name.LocalName.Equals("Text", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        attrs.Add($"{uid}.Text", attr.Value);
                                    }
                                    else if (attr.Name.LocalName.Equals("PlaceholderText", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        attrs.Add($"{uid}.PlaceholderText", attr.Value);
                                    }
                                    else if (attr.Name.LocalName.Equals("Content", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        attrs.Add($"{uid}.Content", attr.Value);
                                    }
                                    else if (attr.Name.LocalName.Equals("NavigationView.PaneTitle", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        attrs.Add($"{uid}.NavigationView.PaneTitle", attr.Value);
                                    }
                                }
                                #endregion

                                if (string.IsNullOrEmpty(uid) && !string.IsNullOrEmpty(name)) uid = name;
                                if (string.IsNullOrEmpty(uid) && string.IsNullOrEmpty(name)) continue;

                                //result = new List<Dictionary<string, string>>() { result, attrs }.SelectMany(dict => dict).ToDictionary(pair => pair.Key, pair => pair.Value);
                                //result = (Dictionary<string, string>)result.Concat(attrs);
                                foreach (var kv in attrs)
                                {
                                    if (result.ContainsKey(kv.Key))
                                    {
                                        var ret = DialogResult.None;
                                        if (rbSkipAll.Checked)
                                            ret = DialogResult.No;
                                        else if (rbReplaceAll.Checked)
                                            ret = DialogResult.Yes;
                                        else if (rbPromptAll.Checked)
                                            ret = MessageBox.Show($"{kv.Key} existed, \n  New: {kv.Key} = {kv.Value}\n  Old: {kv.Key} = {result[kv.Key]}\n. Replace it?", "INFOMATION", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                                        if (ret == DialogResult.Yes) result[kv.Key] = kv.Value;
                                        else if (ret == DialogResult.Cancel) return(result);
                                    }
                                    else
                                    {
                                        result.Add(kv.Key, kv.Value);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine($"{ex.Message}, {lines[0]}");
#endif
                        MessageBox.Show($"{ex.Message}, {lines[0]}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    }
                }
            }
            return (result);
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            string[] param = ParseCommandLine(Environment.CommandLine);
            if (param.Length > 0)
            {
                var files = Directory.GetFiles(param[0], "*.xaml", SearchOption.AllDirectories);

                Dictionary<string, string> stringlist = new Dictionary<string, string>();
                foreach (var file in files)
                {
                    if (file.ToLower().Contains("\\obj\\")) continue;
                    var lines = File.ReadAllLines(file, Encoding.UTF8);
                    var resources = ParseXamlString(lines);
                    //<data name="AppName" xml:space="preserve">
                    //  <value>StringCodec</value>
                    //</data>                   
                    foreach (var kv in resources)
                    {
                        if (stringlist.ContainsKey(kv.Key))
                        {
                            var ret = DialogResult.None;
                            if (rbSkipAll.Checked)
                                ret = DialogResult.No;
                            else if (rbReplaceAll.Checked)
                                ret = DialogResult.Yes;
                            else if(rbPromptAll.Checked) 
                                ret = MessageBox.Show($"{kv.Key} existed, \n\n  New: {kv.Key} = {kv.Value}\n  Old: {kv.Key} = {stringlist[kv.Key]}\n\nReplace it?", "INFOMATION", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                            if (ret == DialogResult.Yes) stringlist[kv.Key] = kv.Value;
                            else if (ret == DialogResult.Cancel) return;
                        }
                        else
                        {
                            stringlist.Add(kv.Key, kv.Value);
                        }
                    }
                }
                StringBuilder sb = new StringBuilder();
                foreach (var kv in stringlist)
                {
                    sb.AppendLine($"  <data name=\"{ kv.Key }\" xml:space=\"preserve\">");
                    sb.AppendLine($"    <value>{ kv.Value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;") }</value>");
                    sb.AppendLine($"  </data>");
                }
                edDst.Text = string.Join("\n", sb);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IronPython.Hosting;
using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Modules;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.IO;
using System.Dynamic;

namespace PythonTest1
{
    public class SharpCalculator
    {
        public double add(double argA, double argB)
        {
            return argA + argB;
        }

        public double sub(double argA, double argB)
        {
            return argA - argB;
        }
    }

    public class DynamicCalc : DynamicObject
    {
        SharpCalculator calc_;
        public DynamicCalc()
        {
            calc_ = new SharpCalculator();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            switch (binder.Name)
            {
                case "add":
                    result = (Func<double, double, double>)((double a, double b) => calc_.add(a, b));
                    return true;
                case "sub":
                    result = (Func<double, double, double>)((double a, double b) => calc_.sub(a, b));
                    return false;
                default:
                    break;
            }
            return false;
        }
    }

    public partial class Form1 : Form
    {
        public string ScriptPath { get; set; }
        public string Arg1 { get; set; }
        public string Arg2 { get; set; }
        public string Arg3 { get; set; }
        public string Arg4 { get; set; }
        public string Arg5 { get; set; }
        public string ScriptName { get; set; }
        public string Result { get; set; }
        public int ExitCode { get; set; }
        public string OtherCode { get; set; }
        public string ExceptionMessage { get; set; }
        public string StdOut { get; set; }

        public Form1()
        {
            InitializeComponent();
        }

        public void Execute()
        {
            string app_path = Directory.GetCurrentDirectory();
            ScriptEngine script_engine = Python.CreateEngine();
            ICollection<string> paths = script_engine.GetSearchPaths();
            paths.Add(app_path);
            script_engine.SetSearchPaths(paths);

            ScriptSource script_source = script_engine.CreateScriptSourceFromFile("Test.py", Encoding.ASCII, SourceCodeKind.File);
            ScriptScope sys = script_engine.GetSysModule();

            var argv = new List { ScriptName, Arg1, Arg2, Arg3, Arg4, Arg5 };
            sys.SetVariable("argv", argv);

            dynamic scope = script_engine.CreateScope();
            scope.form = this;
            scope.proxy = CreateProxy();

            using (var memory_stream = new MemoryStream())
            {
                script_engine.Runtime.IO.SetOutput(memory_stream, new StreamWriter(memory_stream));
                try
                {
                    dynamic thing = script_source.Execute(scope);
                    if (thing != null)
                        Result = thing.ToString();

                    // call def HelloWorld() :
                    var Fnhelloworld = scope.GetVariable<Func<object>>("HelloWorld");
                    string hello_world = Fnhelloworld() as string;
                    MessageBox.Show(hello_world, "1", MessageBoxButtons.OK);

                    var Fnhelloworld3 = scope.GetVariable<Func<object>>("HelloWorld3");
                    Fnhelloworld3();

                    dynamic calc = scope.Calculator();
                    var add_result = calc.add(3, 4);
                    var sub_result = calc.sub(3, 4);

                    var fn_make_login_id = scope.GetVariable<Func<object>>("make_login_id");
                    var login_id = fn_make_login_id();
                    /*
                    // call def HelloWorld2(data) :
                    var Fnhelloworld2 = scope.GetVariable<Func<object, object>>("HelloWorld2");
                    Console.WriteLine(Fnhelloworld2("HelloWorld 2 "));

                    // call def ListTest() :
                    var FnListTest = scope.GetVariable<Func<object>>("ListTest");

                    IronPython.Runtime.List r = (IronPython.Runtime.List)FnListTest();

                    foreach (string data in r)
                    {
                        Console.WriteLine("result: {0}", data);
                    }

                    // call class MyClass
                    var myClass = scope.GetVariable<Func<object, object>>("MyClass");
                    var myInstance = myClass("hello");

                    Console.WriteLine(engine.Operations.GetMember(myInstance, "value"));
                    */
                }
                catch (SystemExitException e)
                {
                    object otherCode;
                    ExitCode = e.GetExitCode(out otherCode);
                    if (otherCode != null)
                        OtherCode = otherCode.ToString();
                }
                catch (Exception e)
                {
                    var eo = script_engine.GetService<ExceptionOperations>();
                    ExceptionMessage = eo.FormatException(e);
                    MessageBox.Show(ExceptionMessage, "Exception", MessageBoxButtons.OK);
                }
                finally
                {
                    var length = (int)memory_stream.Length;
                    var bytes = new byte[length];
                    memory_stream.Seek(0, SeekOrigin.Begin);
                    memory_stream.Read(bytes, 0, length);
                    StdOut = Encoding.UTF8.GetString(bytes, 0, length).Trim();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Execute();
        }

        private object CreateProxy()
        {
            dynamic proxy = new ExpandoObject();
            proxy.ShowMessage = new Action<string>(ShowMessage);
            proxy.Add = new Func<int, int, int>(Add);
            proxy.Random = new Func<int, int, int>(Random);
            return proxy;
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public int Random(int min_value, int max_value)
        {
            Random rnd = new Random();
            return rnd.Next(min_value, max_value);
        }

        public int Add(int a, int b)
        {
            return a + b;
        }
    }
}

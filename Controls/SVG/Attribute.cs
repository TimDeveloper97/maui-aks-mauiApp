using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vst.Controls.SVG
{
    public interface IAttributeConverter
    {
        object GetAttributeValue(Attribute a);
    }
    public class AttributeCollection : Dictionary<string, Attribute>
    {
        public void GetAttribute<T>(string name, Action<T> callback)
            where T : Attribute
        {
            if (TryGetValue(name, out var a))
            {
                callback((T)a);
            }
        }
    }

    public class AttributeCreator : Dictionary<string, Type>
    {
        public AttributeCreator()
        {
            Add("stroke", typeof(ColorAttribute));
            Add("fill", typeof(ColorAttribute));
            Add("d", typeof(DataPathAttribute));
            Add("viewBox", typeof(ViewBoxAttribute));
            Add("points", typeof(PointsAttribute));
        }
        public void Find(string name, Action<Attribute> action)
        {
            Attribute? a = null;
            if (TryGetValue(name, out var type))
            {
                a = Activator.CreateInstance(type) as Attribute;
            }
            else
            {
                a = new NumberAttribute();
            }
            if (a != null)
            {
                a.Name = name;
                action(a);
            }
        }
    }

    public class Attribute
    {
        static public double[] Parse(string input, int startIndex = 0, int endIndex = -1)
        {
            var res = new List<double>();
            long a = 0, b = 0;
            bool? minus = null;

            Action reset = () => { a = b = 0; minus = null; };
            Action add = () =>
            {

                if (minus != null)
                {
                    double v = minus == true ? -a : a;
                    if (b > 1) v /= b;

                    res.Add(v);
                    reset();
                }
            };

            if (endIndex == -1) endIndex = input.Length;

            for (int i = startIndex; i < endIndex; i++)
            {
                char c = input[i];

                if (char.IsDigit(c))
                {
                    if (minus == null) minus = false;

                    a = (a << 1) + (a << 3) + (c & 15);
                    if (b > 0) b = (b << 1) + (b << 3);
                    continue;
                }

                if (char.IsLetter(c))
                {
                    add();
                    break;
                }

                switch (c)
                {
                    case ' ':
                    case ',':
                        add();
                        continue;

                    case '-':
                        add();
                        minus = true;
                        continue;

                    case '.':
                        b = 1;
                        continue;
                }
            }

            add();
            return res.ToArray();
        }

        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

        protected double[]? _numbers;
        public double[] Numbers
        {
            get
            {
                if (_numbers == null)
                {
                    _numbers = Parse(Value);
                }
                return _numbers;
            }
        }

        public virtual string GetShapeName()
        {
            if (Name == "d")
                return "Data";

            var s = new char[Name.Length];
            var last = '-';
            int k = 0;
            for (int i = 0; i < Name.Length; i++)
            {
                var c = Name[i];
                if (c != '-')
                {
                    if (last == '-')
                        c = char.ToUpper(c);
                }
                s[k++] = last = c;
            }
            return new string(s, 0, k);
        }
        public virtual object GetShapeAttributeValue()
        {
            return Value;
        }
        public virtual void ScaleTransform(double scale)
        {
        }
    }
}

namespace Vst.Controls.SVG
{
    public class ColorAttribute : Attribute { }
    public class NumberListAttribute : Attribute {
        public override object GetShapeAttributeValue() => Numbers;
        public override void ScaleTransform(double scale)
        {
            for (int i = 0; i < Numbers.Length; i++)
            {
                Numbers[i] *= scale;
            }
        }
    }
    public class NumberAttribute : NumberListAttribute {
        public override object GetShapeAttributeValue() => Numbers[0];
    }
    public class PointsAttribute : NumberListAttribute { }
    public class ViewBoxAttribute : NumberListAttribute 
    {
        public double Width => Numbers[2];
        public double Height => Numbers[3];
    }
    public class DataPathAttribute : Attribute 
    {
        public class Command
        {
            public char Name { get; set; }
            public double[]? Arguments { get; set; }
            public override string ToString()
            {
                var s = $"{Name}";
                if (Arguments != null)
                    s += string.Join(' ', Arguments);

                return s;
            }
            public virtual void ScaleTransform(double scale)
            {
                if (Arguments != null)
                {
                    if (Name == 'A' || Name == 'a')
                    {
                        int i = 0;
                        while (i < Arguments.Length)
                        {
                            Arguments[i++] *= scale;
                            Arguments[i++] *= scale;

                            i += 3;
                            Arguments[i++] *= scale;
                            Arguments[i++] *= scale;
                        }
                        return;
                    }
                    for (int i = 0; i < Arguments.Length; i++)
                    {
                        Arguments[i] *= scale;
                    }
                }
            }
        }
        public class CommandList : List<Command>
        {
            public override string ToString()
            {
                return string.Join("\r\n", this);
            }
        }

        CommandList? _commands;
        public CommandList Commands
        {
            get
            {
                if (_commands == null)
                {
                    _commands = new CommandList();
                    var pos = new Queue<int>();
                    for (int i = 0; i < Value.Length; i++)
                        if (char.IsLetter(Value[i]))
                            pos.Enqueue(i);

                    pos.Enqueue(Value.Length);
                    int start = pos.Dequeue();

                    while (pos.Count > 0)
                    {
                        int end = pos.Dequeue();
                        _commands.Add(new Command {
                            Name = Value[start],
                            Arguments = Parse(Value, start + 1, end)
                        });

                        start = end;
                    }
                }
                return _commands;
            }
        }
        public override object GetShapeAttributeValue() => Commands.ToString();
        public override void ScaleTransform(double scale)
        {
            foreach (var cmd in Commands)
            {
                cmd.ScaleTransform(scale);
            }
        }
    }
}

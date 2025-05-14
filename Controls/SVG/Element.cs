using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vst.Controls.SVG
{
    public interface IAddChild
    {
        void AppendChild(object child);
    }
    public class ElementCollection : Dictionary<string, Element>
    {
    }
    public class ShapeCreator : Dictionary<string, Type>
    {
    }
    public class AttributeConverter : Dictionary<Type, Func<object, object>>
    {
    }
    public class Element
    {
        static AttributeCreator _attributeCreator = new AttributeCreator();

        public string Name { get; set; } = string.Empty;
        public AttributeCollection Attributes { get; private set; } = new AttributeCollection();

        public void ScaleTransform(double scale)
        {
            foreach (var attribute in Attributes.Values)
            {
                attribute.ScaleTransform(scale);
            }
            if (HasChildren)
            {
                foreach (var e in Children)
                { 
                    e.ScaleTransform(scale); 
                }
            }
        }

        List<Element>? _children;
        public List<Element> Children
        {
            get
            {
                if (_children == null)
                    _children = new List<Element>();
                return _children;
            }
        }
        public bool HasChildren => _children?.Count > 0;

        public void Render(ShapeCreator shapeCreator,
            AttributeConverter attributeConverter, object target)
        {
            var type = target.GetType();
            foreach (var a in Attributes)
            {
                var name = a.Value.GetShapeName();
                var p = type.GetProperty(name);
                if (p == null) continue;

                object v = a.Value.GetShapeAttributeValue();
                if (v.GetType() == p.PropertyType)
                {
                    p.SetValue(target, v, null);
                    continue;
                }

                var atype = a.Value.GetType();
                if (attributeConverter.TryGetValue(atype, out var attrConvert))
                {
                    v = attrConvert(v);
                }
                try
                {
                    p.SetValue(target, v, null);
                    continue;
                }
                catch
                {
                }
                try
                {
                    v = System.Convert.ChangeType(v, p.PropertyType);
                    p.SetValue(target, v, null);
                }
                catch
                {
                }
            }

            if (target is IAddChild && HasChildren)
            {
                foreach (var child in Children)
                {
                    shapeCreator.TryGetValue(child.Name, out type);
                    if (type == null) continue;

                    var s = Activator.CreateInstance(type);
                    if (s != null)
                    {
                        child.Render(shapeCreator, attributeConverter, s);
                        ((IAddChild)target).AppendChild(s);
                    }
                }
            }
        }

        static public Element Parse(Document context)
        {
            var name = context.GetString("t");
            Element elem = new Element {
                Name = name,
            };
            context.GetDocument("a", attrs => {
                foreach (var attr in attrs)
                {
                    _attributeCreator.Find(attr.Key, a => {
                        a.Value = attr.Value?.ToString() ?? string.Empty;
                        elem.Attributes.Add(attr.Key, a);
                    });
                }
            });
                    
            var childs = context.GetArray<Document>("c");
            if (childs.Count > 0)
            {
                foreach (var child in childs)
                {
                    elem.Children.Add(Parse(child));
                }
            }
            return elem;
        }
        static public Element Parse(Stream stream)
        {
            using var sr = new StreamReader(stream);
            var context = Document.Parse(sr.ReadToEnd());

            return Element.Parse(context);
        }
    }
}

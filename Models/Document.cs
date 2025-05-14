using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace System
{
    using MAP = Dictionary<string, object>;
    public partial class Document : MAP
    {
        public Document() : this(null) { }
        public Document(MAP? data)
        {
            if (data != null) 
            {
                foreach (var p in data)
                {
                    if (p.Value != null)
                    {
                        Add(p.Key, p.Value);
                    }
                }
            }
        }

        public static Document FromObject(object? obj)
        {
            if (obj == null) return new Document();
            var type = obj.GetType();
            
            if (type == typeof(Document)) return (Document)obj;
            if (type == typeof(MAP)) return new Document((MAP)obj);

            if (type == typeof(JsonElement))
            {
                return ((JsonElement)obj).Deserialize<Document>() ?? new Document();
            }

            var doc = new Document();
            foreach (var p in type.GetProperties())
            {
                var v = p.GetValue(obj);
                if (v == null) continue;

                doc.Add(p.Name, v);
            }
            return doc;
        }
        public static Document Parse(string json)
        {
            return JsonSerializer.Deserialize<Document>(json) ?? new Document();
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

        new public object? this[string key]
        {
            set
            {
                if (value != null)
                {
                    base[key] = value;
                }
                else
                {
                    base.Remove(key);
                }
            }
            get
            {
                base.TryGetValue(key, out var value);
                return value;
            }
        }

        #region GET / SET
        public void SetValue(string key, object? value)
        {
            if (value == null) 
            { 
                Remove(key); 
                return;
            }
            base[key] = value;
        }
        public string GetString(string key)
        {
            var value = this[key];
            return value?.ToString() ?? string.Empty;
        }
        public DateTime? GetDateTime(string key)
        {
            var value = this[key];
            if (value == null) return null;

            var type = value.GetType();
            if (type == typeof(DateTime))
                return (DateTime)value;

            if (type == typeof(long))
                return new DateTime((long)value);

            if (type == typeof(string))
            {
                var d = DateTime.Now;
                var v = new int[] { d.Year, d.Month, d.Day, 0, 0, 0 };
                int i = 0, a = 0;
                foreach (var c in (string)value)
                {
                    if (char.IsDigit(c))
                    {
                        a = (a << 1) + (a << 3) + (c & 15);
                        continue;
                    }
                    v[i++] = a;
                    if (i == v.Length) break;

                    a = 0;
                }

                return new DateTime(v[0], v[1], v[2], v[3], v[4], v[5]);
            }
            return null;
        }
        public T? GetValue<T>(string key)
        {
            var v = this[key];
            if (v == null) return default;
            return (T)Convert.ChangeType(v, typeof(T));
        }
        public List<T> GetArray<T>(string key)
        {
            var v = this[key];
            if (v != null)
            {
                if (v is List<T>)
                    return (List<T>)v;

                if (v is string)
                {
                    var lst = new List<T>();
                    var s = (string)v;
                    var src = JsonArray.Parse(s)?.AsArray();
                    if (src == null) return lst;

                    foreach (var node in src)
                    {
                        if (node != null)
                        {
                            var a = node.GetValue<T>();
                            lst.Add(a);
                        }
                    }
                    return lst;
                }

                if (v is JsonElement)
                {
                    var lst = ((JsonElement)v).Deserialize<List<T>>();
                    if (lst != null) return lst;
                }
            }


            return new List<T>();
        }
        public Document GetDocument(string key)
        {
            return FromObject(this[key]);
        }
        public Document GetDocument(string key, Action<Document> callback)
        {
            var v = this[key];
            Document doc = FromObject(v);
            if (v == null)
            {
                base.Add(key, doc);
            }
            if (callback != null)
                callback(doc);
            return doc;
        }
        #endregion

        #region KEYS
        public struct DocumentKeys
        {
            public const string ObjectId = "_id";
            public const string Value = "value";
        }
        #endregion

        #region Attributes
        public object? ObjectId { get => this[DocumentKeys.ObjectId]; set => this[DocumentKeys.ObjectId] = value; }
        public object? Value { get => this[DocumentKeys.Value]; set => this[DocumentKeys.Value] = value; }
        #endregion
    }
}

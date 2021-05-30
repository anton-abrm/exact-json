using System;
using System.IO;
using System.Text;

namespace ExactJson.Serialization
{

    public sealed partial class JsonSerializer
    {
        public JsonNodeSerializationContext GetContext<T>()
        {
            return GetContext(typeof(T));
        }

        public void SetContext<T>(JsonNodeSerializationContext context)
        {
            SetContext(typeof(T), context);
        }

        public void SetupContext<T>(Action<JsonNodeSerializationContext> setup)
        {
            if (setup is null) {
                throw new ArgumentNullException(nameof(setup));
            }

            var context = GetContext<T>();
            if (context is null) {
                context = new JsonNodeSerializationContext();
                SetContext<T>(context);
            }

            setup(context);
        }

        #region Serialize

        public void Serialize<T>(JsonWriter writer, object value)
            => Serialize<T>(writer, value, null);

        public void Serialize<T>(JsonWriter writer, object value, JsonNodeSerializationContext context)
            => Serialize(typeof(T), writer, value, context);

        public string Serialize<T>(object value)
            => Serialize<T>(value, null);

        public string Serialize<T>(object value, JsonNodeSerializationContext context)
            => Serialize(typeof(T), value, context);

        public void Serialize(Type type, JsonWriter writer, object value)
            => Serialize(type, writer, value, null);
        
        public string Serialize(Type type, object value)
            => Serialize(type, value, null);

        public string Serialize(Type type, object value, JsonNodeSerializationContext context)
        {
            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            using (var jw = new JsonTextWriter(sw)) {
                Serialize(type, jw, value, context);
            }

            return sb.ToString();
        }

        #endregion

        #region Deserialize

        public object Deserialize(Type type, string json, JsonNodeSerializationContext context = null)
        {
            using (var jr = new JsonStringReader(json)) {
                return Deserialize(type, jr, context);
            }
        }

        public T Deserialize<T>(string json)
            => Deserialize<T>(json, null);

        public T Deserialize<T>(string json, JsonNodeSerializationContext context)
            => (T) Deserialize(typeof(T), json, context);

        public T Deserialize<T>(JsonReader reader)
            => Deserialize<T>(reader, null);

        public T Deserialize<T>(JsonReader reader, JsonNodeSerializationContext context)
            => (T) Deserialize(typeof(T), reader, context);

        public void RegisterType<T>(string alias)
            => RegisterType(typeof(T), alias);

        #endregion
    }

}
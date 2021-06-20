using System;

namespace ExactJson
{
    public abstract class JsonWriter : IDisposable
    {
        public abstract void WriteProperty(string name);
        public abstract void WriteNull();
        public abstract void WriteBool(bool value);
        public abstract void WriteNumber(JsonDecimal value, JsonNumberFormat format);
        public abstract void WriteString(string value);
        public abstract void WriteStartObject();
        public abstract void WriteEndObject();
        public abstract void WriteStartArray();
        public abstract void WriteEndArray();

        #region WriteNumber

        public virtual void WriteNumber(double value, string format = null)
        {
            WriteNumber((JsonDecimal) value, format is not null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        public virtual void WriteNumber(float value, string format = null)
        {
            WriteNumber((JsonDecimal) value, format is not null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        public virtual void WriteNumber(decimal value, string format = null)
        {
            WriteNumber(value, format is not null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        [CLSCompliant(false)]
        public virtual void WriteNumber(sbyte value, string format = null)
        {
            WriteNumber(value, format is not null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        public virtual void WriteNumber(byte value, string format = null)
        {
            WriteNumber(value, format is not null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        [CLSCompliant(false)]
        public virtual void WriteNumber(ushort value, string format = null)
        {
            WriteNumber(value, format is not null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        public virtual void WriteNumber(short value, string format = null)
        {
            WriteNumber(value, format is not null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        [CLSCompliant(false)]
        public virtual void WriteNumber(uint value, string format = null)
        {
            WriteNumber(value, format is not null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        public virtual void WriteNumber(int value, string format = null)
        {
            WriteNumber(value, format is not null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        [CLSCompliant(false)]
        public virtual void WriteNumber(ulong value, string format = null)
        {
            WriteNumber(value, format is not null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        public virtual void WriteNumber(long value, string format = null)
        {
            WriteNumber(value, format is not null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value));
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        { }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
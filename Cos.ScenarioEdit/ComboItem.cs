﻿namespace ScenarioEdit
{
    public class ComboItem<T>
    {
        public string Text { get; set; }
        public T Value { get; set; }

        public ComboItem(T value)
        {
            Text = value?.ToString() ?? string.Empty;
            Value = value;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}

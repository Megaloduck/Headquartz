using System.Globalization;

namespace Headquartz.Converters
{
    public class BoolToSpeedButtonColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected && isSelected)
            {
                return Application.Current.Resources.TryGetValue("PrimaryColor", out var color)
                    ? color
                    : Colors.Blue;
            }
            return Application.Current.Resources.TryGetValue("SecondaryButtonBackground", out var bgColor)
                ? bgColor
                : Colors.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToSpeedButtonTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected && isSelected)
            {
                return Colors.White;
            }
            return Application.Current.Resources.TryGetValue("PrimaryTextColor", out var color)
                ? color
                : Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToPhaseColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive && isActive)
            {
                return Application.Current.Resources.TryGetValue("PrimaryColor", out var color)
                    ? color
                    : Colors.Blue;
            }
            return Application.Current.Resources.TryGetValue("SurfaceColor", out var bgColor)
                ? bgColor
                : Colors.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive && isActive)
            {
                return Colors.White;
            }
            return Application.Current.Resources.TryGetValue("SecondaryTextColor", out var color)
                ? color
                : Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ProgressToXConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double progress)
            {
                return $"{progress},0.5,0,0";
            }
            return "0,0.5,0,0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToEventIndicatorColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool hasEvent && hasEvent)
            {
                return Application.Current.Resources.TryGetValue("PrimaryColor", out var color)
                    ? color
                    : Colors.Blue;
            }
            return Application.Current.Resources.TryGetValue("SurfaceColor", out var bgColor)
                ? bgColor
                : Colors.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToDayTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool hasEvent && hasEvent)
            {
                return Colors.White;
            }
            return Application.Current.Resources.TryGetValue("PrimaryTextColor", out var color)
                ? color
                : Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
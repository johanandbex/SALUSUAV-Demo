using System;

namespace SALUSUAV_Demo.Extensions
{
    /// <summary>
    /// A latitude or longitude coordinate
    /// </summary>
    public abstract class Coordinate
    {
        #region Public Properties

        /// <summary>
        /// The number of degrees in the coordinate
        /// </summary>
        public int Degrees { get; }

        /// <summary>
        /// The number of minutes in the coordinate
        /// </summary>
        public int Minutes { get; }

        /// <summary>
        /// The number of seconds in the coordinate
        /// </summary>
        public int Seconds { get; }

        /// <summary>
        /// The type of this coordinate
        /// </summary>
        public CoordinateType Type { get; }

        /// <summary>
        /// The max value for degrees for this DMS
        /// </summary>
        public int MaxDegrees { get; }

        /// <summary>
        /// The min value for degrees for this DMS
        /// </summary>
        public int MinDegrees { get; }

        /// <summary>
        /// The coordinate represented as a degree decimal
        /// </summary>
        public double DecimalDegrees { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Coordinate object
        /// </summary>
        /// <param name="degrees">The degrees in the coordinate</param>
        /// <param name="minutes">The minutes in the coordinate</param>
        /// <param name="seconds">The seconds in the coordinate</param>
        /// <param name="type">The coordinate type</param>
        protected Coordinate(int degrees, int minutes, int seconds, CoordinateType type)
        {
            if (minutes >= 60 || minutes < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minutes), "Minutes must be less than 60 and cannot be negative.");
            }

            if (seconds >= 60 || seconds < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(seconds), "Seconds must be less than 60 and cannot be negative.");
            }

            switch (type)
            {
                case CoordinateType.Latitude:
                    {
                        if ((degrees > 90 || degrees < -90) ||
                            ((degrees == 90 || degrees == -90) && minutes != 0 && seconds != 0))
                        {
                            throw new ArgumentOutOfRangeException("A latitude must be between -90 and 90 degrees", new Exception("A latitude must be between -90 and 90 degrees"));
                        }

                        MaxDegrees = 90;
                        MinDegrees = -90;

                        break;
                    }
                case CoordinateType.Longitude:
                    {
                        if ((degrees > 180 || degrees < -180) ||
                           ((degrees == 180 || degrees == -180) && minutes != 0 && seconds != 0))
                        {
                            throw new ArgumentOutOfRangeException("A longitude must be between -180 and 180 degrees", new Exception("A longitude must be between -180 and 180 degrees"));
                        }

                        MaxDegrees = 180;
                        MinDegrees = -180;

                        break;
                    }
                default:
                    {
                        throw new ArgumentException($"The type {type} is unknown.");
                    }
            }

            Degrees = degrees;
            Minutes = minutes;
            Seconds = seconds;
            Type = type;
            DecimalDegrees = Degrees + (Minutes / 60) + (Seconds / 3600);
        }

        /// <summary>
        /// Creates a new Coordinate object
        /// </summary>
        /// <param name="decimalDegrees">The decimal degrees representation of the coordinate</param>
        /// <param name="type">The coordinate type</param>
        protected Coordinate(double decimalDegrees, CoordinateType type)
        {
            DecimalDegrees = decimalDegrees;
            Type = type;

            switch (Type)
            {
                case CoordinateType.Latitude:
                    {
                        if (DecimalDegrees > 90 || DecimalDegrees < -90)
                        {
                            throw new ArgumentOutOfRangeException(nameof(decimalDegrees), "The decimal degrees cannot be greather than 90 or less than -90.");
                        }

                        MaxDegrees = 90;
                        MinDegrees = -90;

                        break;
                    }
                case CoordinateType.Longitude:
                    {
                        if (DecimalDegrees > 180 || DecimalDegrees < -180)
                        {
                            throw new ArgumentOutOfRangeException(nameof(decimalDegrees), "The decimal degrees cannot be greather than 180 or less than -180.");
                        }

                        MaxDegrees = 180;
                        MinDegrees = -180;

                        break;
                    }
                default:
                    {
                        throw new ArgumentException($"The type {type} is unknown.");
                    }
            }

            Degrees = (int)Math.Floor(DecimalDegrees);
            Minutes = (int)Math.Floor(60 * (DecimalDegrees - Degrees));
            Seconds = (int)((3600 * (DecimalDegrees - Degrees)) - (60 * Minutes));
        }

        #endregion

        #region Public Methods

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            // ReSharper disable once SuggestVarOrType_SimpleTypes
            Coordinate other = (Coordinate)obj;

            return Degrees.Equals(other.Degrees) &&
                Minutes.Equals(other.Minutes) &&
                Seconds.Equals(other.Seconds) &&
                Type.Equals(other.Type);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return 17 * (Degrees.GetHashCode() + Minutes.GetHashCode() + Seconds.GetHashCode() + Type.GetHashCode());
            }
        }

        public override string ToString()
        {
            // ReSharper disable once SuggestVarOrType_BuiltInTypes
            char dir;

            switch (Type)
            {
                case CoordinateType.Latitude:
                {
                    dir = Degrees >= 0 ? 'N' : 'S';
                    break;
                }
                case CoordinateType.Longitude:
                {
                    dir = Degrees >= 0 ? 'W' : 'E';
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return ($"{Degrees}° {Minutes}' {Seconds}\" {dir}").Trim();
        }

        #endregion 
    }
}

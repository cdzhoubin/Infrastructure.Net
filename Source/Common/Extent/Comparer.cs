using System;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Zhoubin.Infrastructure.Common.Extent
{
    /// <summary>
    /// 来源：
    /// http://www.codeproject.com/Articles/10683/Object-Comparer
    /// </summary>
    class Comparer
    {
        protected Regex regex;

        protected ArrayList precedence = new ArrayList(new string[]
                {
                    "System.Boolean",
                    "System.Byte",
                    "System.Sbyte",
                    "System.Char",
                    "System.UInt16",
                    "System.Int16",
                    "System.UInt32",
                    "System.Int32",
                    "System.UInt64",
                    "System.Int64",
                    "System.Decimal",
                    "System.Float",
                    "System.Double",
                }
            );

        protected ArrayList unsignedTypes = new ArrayList(new string[]
                {
                    "System.Byte",
                    "System.UInt16",
                    "System.UInt32",
                    "System.UInt64",
                }
            );

        public Comparer()
        {
            // This expression:
            // Matches any optional + - . , characters at the beginning
            // Followed by 1 or digits
            // Followed by an optional . or ,
            // Followed by an optional e or E
            // Followed by an optional + - . ,
            // Followed by 1 or more digits
            // Followed by an optional . or ,
            // Followed by 0 or more digits

            regex = new Regex(@"[+-\.,]?\d*[\.,]?\d*e?[+-\.,]?\d+[\.,]?\d*",
                RegexOptions.CultureInvariant |
                RegexOptions.IgnoreCase |
                RegexOptions.Singleline);
        }

        public int Compare(object a, object b)
        {
            bool success = false;
            object a2, b2;
            int ret = 0;
            Type typea = a.GetType();
            Type typeb = b.GetType();

            if (typea.FullName == "System.Boolean")
            {
                a = Convert.ToInt32(a);
                typea = typeof(System.Int32);
            }

            if (typeb.FullName == "System.Boolean")
            {
                b = Convert.ToInt32(b);
                typeb = typeof(System.Int32);
            }

            // Does a implement IComparable?
            if (typea.GetInterface("IComparable") == null)
            {
                // Does b implement IComparable?
                if (typeb.GetInterface("IComparable") == null)
                {
                    // If neither implements IComparable,
                    // then fall back to a string comparison.
                    ret = CompareAsStrings(a, b);
                }
                else
                {
                    // Do the comparison using b as the "compare with",
                    // then invert the result.
                    ret = Compare(b, a) * -1;
                }
            }
            else
            {
                // Verify that a and b are the same type, but not strings.
                if ((typea.AssemblyQualifiedName == typeb.AssemblyQualifiedName) && (!(a is String)))
                {
                    // They are the same type.
                    ret = ((IComparable)a).CompareTo(b);
                }
                else
                {
                    // Attempt to convert b to a's type, or a to b's type,
                    // paying attention to the fact that we want to convert
                    // to a string as a last resort.

                    if ((a is String) || (b is String))
                    {
                        // If a is a string and b implements IComparable...
                        if ((a is String) && (typeb.GetInterface("IComparable") != null))
                        {
                            bool stringIsNumeric = TestForNumeric(ref a, ref typea);
                            if (b is String)
                            {
                                TestForNumeric(ref b, ref typeb);
                            }
                            if (stringIsNumeric)
                            {
                                ret = Compare(a, b);
                            }
                            else
                            {
                                // Attempt to get a as a type of b.
                                success = MatchTypesToB(a, b, typea, typeb, out a2, out b2);
                                if (success)
                                {
                                    ret = ((IComparable)a2).CompareTo(b2);
                                }
                                else
                                {
                                    ret = CompareAsStrings(a, b);
                                }
                            }
                        }
                        else
                        {
                            bool stringIsNumeric = TestForNumeric(ref b, ref typeb);
                            if (stringIsNumeric)
                            {
                                ret = Compare(a, b);
                            }
                            else
                            {
                                // Attempt to get b as a type of a.
                                // We know that a implements IComparable.
                                success = MatchTypesToA(a, b, typea, typeb, out a2, out b2);
                                if (success)
                                {
                                    ret = ((IComparable)a2).CompareTo(b2);
                                }
                                else
                                {
                                    CompareAsStrings(a, b);
                                }
                            }
                        }
                    }
                    else
                    {
                        // If neither is a string...
                        // Are they both value types?
                        if ((typea.IsValueType) && (typeb.IsValueType))
                        {
                            // Then attempt to convert to the highest precision type.
                            success = MatchTypesByPrecision(a, b, typea, typeb, out a2, out b2);
                            if (success)
                            {
                                ret = ((IComparable)a2).CompareTo(b2);
                            }
                            else
                            {
                                // Compare as strings.
                                ret = CompareAsStrings(a, b);
                            }
                        }
                        else
                        {
                            // Try converting b to a.
                            success = MatchTypesToA(a, b, typea, typeb, out a2, out b2);
                            if (success)
                            {
                                // b2 is now a's type.
                                ret = ((IComparable)a2).CompareTo(b2);
                            }
                            else
                            {
                                if (typeb.GetInterface("IComparable") != null)
                                {
                                    // b also implements IComparable.
                                    success = MatchTypesToB(a, b, typea, typeb, out a2, out b2);
                                    if (success)
                                    {
                                        // a was successfully converted to a "b".
                                        ret = ((IComparable)a2).CompareTo(b2);
                                    }
                                    else
                                    {
                                        // Fall back to string comparisons.
                                        ret = CompareAsStrings(a, b);
                                    }
                                }
                                else
                                {
                                    // Fall back to string comparisons.
                                    ret = CompareAsStrings(a, b);
                                }
                            }
                        }
                    }
                }
            }
            return ret;
        }

        protected int CompareAsStrings(object a, object b)
        {
            return a.ToString().CompareTo(b.ToString());
        }

        protected bool MatchTypesByPrecision(object a, object b, Type typea, Type typeb, out object a2, out object b2)
        {
            a2 = a;
            b2 = b;
            bool ret = false;

            if (a is Enum)
            {
                a = Convert.ToDouble(a);
                typea = typeof(System.Double);
            }

            if (b is Enum)
            {
                b = Convert.ToDouble(b);
                typeb = typeof(System.Double);
            }

            int uidxa = unsignedTypes.IndexOf(typea.FullName);
            int uidxb = unsignedTypes.IndexOf(typeb.FullName);

            if ((uidxa != -1) && (uidxb == -1))
            {
                a = Convert.ToDecimal(a);
                typea = typeof(System.Decimal);
            }
            else if ((uidxb != -1) && (uidxa == -1))
            {
                b = Convert.ToDecimal(b);
                typeb = typeof(System.Decimal);
            }

            int idxa = precedence.IndexOf(typea.FullName);
            int idxb = precedence.IndexOf(typeb.FullName);
            if (idxa < idxb)
            {
                ret = MatchTypesToB(a, b, typea, typeb, out a2, out b2);
            }
            else if (idxa > idxb)
            {
                ret = MatchTypesToA(a, b, typea, typeb, out a2, out b2);
            }
            else
            {
                a2 = a;
                b2 = b;
                ret = true;
            }

            return ret;
        }

        protected bool MatchTypesToA(object a, object b, Type typea, Type typeb, out object a2, out object b2)
        {
            bool ret = false;
            a2 = a;
            b2 = b;
            // Try convert to.
            TypeConverter tcb = TypeDescriptor.GetConverter(typeb);
            if (tcb.CanConvertTo(typea))
            {
                b2 = tcb.ConvertTo(b, typea);
                ret = true;
            }
            else
            {
                // Try convert from.
                TypeConverter tca = TypeDescriptor.GetConverter(typea);
                if (tca.CanConvertFrom(typeb))
                {
                    b2 = tca.ConvertFrom(b);
                    ret = true;
                }
            }
            return ret;
        }

        protected bool MatchTypesToB(object a, object b, Type typea, Type typeb, out object a2, out object b2)
        {
            bool ret = false;
            a2 = a;
            b2 = b;

            // Try convert to.
            TypeConverter tca = TypeDescriptor.GetConverter(typea);
            if (tca.CanConvertTo(typeb))
            {
                a2 = tca.ConvertTo(a, typeb);
                ret = true;
            }
            else
            {
                // Try convert from.
                TypeConverter tcb = TypeDescriptor.GetConverter(typeb);
                if (tcb.CanConvertFrom(typea))
                {
                    a2 = tcb.ConvertFrom(a);
                    ret = true;
                }
            }
            return ret;
        }

        protected bool TestForNumeric(ref object a, ref Type typea)
        {
            Match match = regex.Match(a.ToString());
            bool ret = match.Value == a.ToString();
            if (ret)
            {
                a = Convert.ToDouble(a);
                typea = typeof(System.Double);
            }
            return ret;
        }
    }
}

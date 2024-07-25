﻿/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.ComponentModel;
using System.Net.Mail;
using System.Reflection;

namespace Inspector.Infra.Utils
{
    /// <summary>
    /// EnumDescription
    /// </summary>
    public static class EnumDescription
    {
        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerationValue">The enumeration value.</param>
        /// <returns></returns>
        public static string GetDescription<T>(this T enumerationValue) where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }

        public static T ToEnum<T>(this string enumerationDescription) 
        {

            if (string.IsNullOrEmpty(enumerationDescription))
            {
                throw new ArgumentNullException("enumerationDescription");
            }

            //remove special chars:
            enumerationDescription = "Item" +
                enumerationDescription.Replace("(", "").Replace("-", "").Replace(")", "").Replace("°", "").Replace("/", "").Replace(".", "");
            //cast to the enum
            try
            {
                var result = Enum.Parse(typeof (T), enumerationDescription, true);
                return (T)result;
            }
            catch
            {
                throw new ArgumentException("The specified type was not an enum type");
            }

        }

    }
}

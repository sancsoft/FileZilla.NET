/******************************************************************************
 * Filename: WebConvert.cs
 * Project:  FileZilla.NET
 * 
 * Description:
 * Web-safe conversion utilities that do not fail (return a default value)
 * 
 * Revision History:
 * $Log: /FileZilla.NET/website/App_Code/WebConvert.cs $
 * 
 * 1     2/26/12 12:20p Mterry
******************************************************************************/
using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

/// <summary>
/// The WebConvert class is a collection of conversion routines designed for use with the web.
/// </summary>
public static class WebConvert
{
	/// <summary>
	/// Attempt to convert the supplied object to an Int32.
	/// </summary>
	/// <param name="input">The object to parse.</param>
	/// <param name="defaultValue">The default value to use if the conversion fails.</param>
	/// <returns>The converted value if the conversion passes, otherwise the default value.</returns>
	public static int ToInt32( object input, int defaultValue )
	{
		int ret = defaultValue;

		if ( input != null )
		{
			if ( !Int32.TryParse( input.ToString(), out ret ) )
			{
				ret = defaultValue;
			}
		}

		return ret;
	}

	/// <summary>
	/// Attempt to convert the supplied object to a single-precision float.
	/// </summary>
	/// <param name="input">The object to parse.</param>
	/// <param name="defaultValue">The default value to use if the conversion fails.</param>
	/// <returns>The converted value if the conversion passes, otherwise the default value.</returns>
	public static float ToSingle( object input, float defaultValue )
	{
		float ret = defaultValue;

		if ( input != null )
		{
			if ( !Single.TryParse( input.ToString(), out ret ) )
			{
				ret = defaultValue;
			}
		}

		return ret;
	}

	/// <summary>
	/// Attempt to convert the supplied object to a double-precision float.
	/// </summary>
	/// <param name="input">The object to parse.</param>
	/// <param name="defaultValue">The default value to use if the conversion fails.</param>
	/// <returns>The converted value if the conversion passes, otherwise the default value.</returns>
	public static double ToDouble( object input, double defaultValue )
	{
		double ret = defaultValue;

		if ( input != null )
		{
			if ( !Double.TryParse( input.ToString(), out ret ) )
			{
				ret = defaultValue;
			}
		}

		return ret;
	}

	/// <summary>
	/// Attempt to convert the supplied object to a String.
	/// </summary>
	/// <param name="input">The object to parse.</param>
	/// <param name="defaultValue">The default value to use if the conversion fails.</param>
	/// <returns>The converted value if the conversion passes, otherwise the default value.</returns>
	public static string ToString( object input, string defaultValue )
	{
		string ret = defaultValue;

		if ( input != null )
		{
			if ( input.ToString().Length > 0 )
			{
				ret = input.ToString();
			}
		}
		
		return ret;
	}

	/// <summary>
	/// Attempt to convert the supplied object to a Boolean.
	/// </summary>
	/// <param name="input">The object to parse.</param>
	/// <param name="defaultValue">The default value to use if the conversion fails.</param>
	/// <returns>The converted value if the conversion passes, otherwise the default value.</returns>
	public static bool ToBoolean( object input, bool defaultValue )
	{
		bool ret = defaultValue;

		if ( input != null )
		{
			if ( !Boolean.TryParse( input.ToString(), out ret ) )
			{
				ret = defaultValue;
			}
		}

		return ret;
	}

    /// <summary>
    /// Attempt to convert the supplied object to a DateTime.
    /// </summary>
    /// <param name="input">The object to parse.</param>
    /// <param name="defaultValue">The default value to use if the conversion fails.</param>
    /// <returns>The converted value if the conversion passes, otherwise the default value.</returns>
    public static DateTime ToDateTime( object input, DateTime defaultValue )
    {
        DateTime ret = defaultValue;

        if ( input != null )
        {
            if ( !DateTime.TryParse( input.ToString(), out ret ) )
            {
                ret = defaultValue;
            }
        }

        return ret;
    }
}
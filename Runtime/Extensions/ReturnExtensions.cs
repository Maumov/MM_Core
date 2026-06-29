using System.Collections;

namespace MM.Extensions
{
public static class ReturnExtensions
{
	public static int ResultToInt(this object ao_Object)
	{
		if(ao_Object is int)
		{
			return (int)ao_Object;
		}
		else if(ao_Object is string)
		{
			int valeur = 0;
			try
			{
				valeur = int.Parse((string)ao_Object);
			}
			catch
			{
			}
			return valeur;
		}
		else if(ao_Object is double)
		{
			return (int)(double)ao_Object;
		}
		else if(ao_Object is uint)
		{
			return (int)(uint)ao_Object;
		}
		else if(ao_Object is long)
		{
			return (int)(long)ao_Object;
		}
		else if(ao_Object is ulong)
		{
			return (int)(ulong)ao_Object;
		}
		return int.MinValue;
	}
	
	public static string ResultToString(this object ao_Object)
	{
		if(ao_Object is string)
		{
			return (string)ao_Object;
		}
		else if(ao_Object is double)
		{
			return ((double)ao_Object).ToString();
		}
		return null;
	}
	
	public static string[] ResultToStringArray(this object ao_Object)
	{
		if(ao_Object is string)
		{
			string str = (string)ao_Object;
			int nbElements = 1;
			
			for(int i = 0;i< str.Length;i++)
			{
				if (str[i] == ',')
				{
					nbElements++;
				}
			}
			
			string[] array = new string[nbElements];
			
			int index = 0;
			for(int i = 0;i< nbElements;i++)
			{
				index = (str.IndexOf(",") == -1) ? str.Length: str.IndexOf(",");
				
				array[i] = str.Substring(0,index);
				
				if(str.IndexOf(",") != -1)
				{
					str = str.Remove(0,index+1);
				}
			}
			return array;
		}
		else if(ao_Object is ArrayList)
		{
			ArrayList array = (ArrayList)ao_Object;
			string[] str = null;
			
			int nbElements = array.Count;
			//			int nbElements = 0;
			//			foreach(ao_Objectect o in array)
			//			{
			//				nbElements++;
			//			}
			
			str = new string[nbElements];
			
			for(int i = 0;i<nbElements;i++)
			{
				str[i] = array[i].ToString();
			}
			
			return str;
		}
		
		return null;
	}
	
	public static float ResultToFloat(this object ao_Object)
	{
		if(ao_Object is float)
		{
			return (float)ao_Object;
		}
		else if(ao_Object is double)
		{
			return (float)(double)ao_Object;
		}
		else if(ao_Object is string)
		{
			float valeur = 0f;
			try
			{
				valeur = float.Parse((string)ao_Object);
			}
			catch
			{
			}
			return valeur;
		}
		else if(ao_Object is uint)
		{
			return (float)(uint)ao_Object;
		}
		else if(ao_Object is System.UInt32)
		{
			return (float)(System.UInt32)ao_Object;
		}
		return float.NaN;
	}
	
	public static double ResultToDouble(this object ao_Object)
	{
		if(ao_Object is float)
		{
			return (double)(float)ao_Object;
		}
		else if(ao_Object is double)
		{
			return (double)ao_Object;
		}
		else if(ao_Object is string)
		{
			double valeur = 0;
			try
			{
				valeur = double.Parse((string)ao_Object);
			}
			catch
			{
			}
			return valeur;
		}
		return double.NaN;
	}
	
	public static uint ResultToUint(this object ao_Object)
	{
		if(ao_Object is uint)
		{
			return (uint)ao_Object;
		}
		else if(ao_Object is double)
		{
			return (uint)(double)ao_Object;
		}
		else if(ao_Object is string)
		{
			uint valeur = 0;
			try
			{
				valeur = uint.Parse((string)ao_Object);
			}
			catch
			{
			}
			return valeur;
		}
		return 0;
	}
	
	public static long ResultToLong(this object ao_Object)
	{
		if(ao_Object is long)
		{
			return (long)ao_Object;
		}
		else if(ao_Object is double)
		{
			return (long)(double)ao_Object;
		}
		else if(ao_Object is string)
		{
			long valeur = 0;
			try
			{
				valeur = long.Parse((string)ao_Object);
			}
			catch
			{
			}
			return valeur;
		}
		return 0;
	}
	
	public static ulong ResultToULong(this object ao_Object)
	{
		if(ao_Object is ulong)
		{
			return (ulong)ao_Object;
		}
		else if(ao_Object is double)
		{
			return (ulong)(double)ao_Object;
		}
		else if(ao_Object is string)
		{
			ulong valeur = 0;
			try
			{
				valeur = ulong.Parse((string)ao_Object);
			}
			catch
			{
			}
			return valeur;
		}
		return 0;
	}
	
	public static bool ResultToBool(this object ao_Object)
	{
		if(ao_Object is bool)
		{
			return (bool)ao_Object;
		}
		else if(ao_Object is double)
		{
			return ((double)ao_Object != 0);
		}
		else if(ao_Object is string)
		{
			bool valeur = false;
			try
			{
				valeur = bool.Parse((string)ao_Object);
			}
			catch
			{
			}
			return valeur;
		}
		return false;
	}
}
}

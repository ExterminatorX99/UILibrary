using System;
using System.Reflection;

namespace UILibrary
{
	public static class Reflection
	{
		public static TDelegate Delegate<TType, TDelegate>(string name, BindingFlags bindingAttr) where TDelegate : Delegate =>
			typeof(TType).GetMethod(name, bindingAttr)!.CreateDelegate<TDelegate>();

		public static TDelegate Delegate<TDelegate>(Type type, string name, BindingFlags bindingAttr) where TDelegate : Delegate =>
			type.GetMethod(name, bindingAttr)!.CreateDelegate<TDelegate>();
	}
}

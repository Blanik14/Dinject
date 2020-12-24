using Dinject.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Dinject.Core
{
	public class ImplementationType
	{
		public Type Type { get; private set; }

		public IList<ParameterInfo> Parameters { get; private set; }

		public ImplementationType(Type type)
		{
			this.Type = type;
			this.Initialize(this.Type);
		}

		private void Initialize(Type type)
		{
			this.Parameters = this.GetParameters(this.GetDIConstructor(type));
		}

		private ConstructorInfo GetDIConstructor(Type type)
		{
			ConstructorInfo usedConstructor = null;
			var constructors = type.GetConstructors();

			if (constructors.Length > 1)
			{
				foreach (var constructor in constructors)
				{
					var attribute = constructor.GetCustomAttributes<DIConstructorAttribute>();
					if (attribute != default(DIConstructorAttribute))
					{
						usedConstructor = constructor;
					}
				}
			}
			else
			{
				usedConstructor = constructors[0];
			}

			if (usedConstructor == null)
			{
				throw new Exception("DIConstructor not found");
			}

			return usedConstructor;
		}

		private IList<ParameterInfo> GetParameters(ConstructorInfo constructorInfo)
		{
			var parameters = constructorInfo.GetParameters();
			return parameters.OrderBy(p => p.Position).ToList();
		}
	}

	public class ImplementationType<T> : ImplementationType where T : class
	{
		public ImplementationType() 
			:base(typeof(T))
		{
		}

		
	}
}

using Dinject.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dinject
{
	public class Container
	{
		private IDictionary<Type, ImplementationType> _registeredTypes;

		private IDictionary<Type, ImplementationType> _registeredDependencyTypes;

		public Container()
		{
			this._registeredTypes = new Dictionary<Type, ImplementationType>();
			this._registeredDependencyTypes = new Dictionary<Type, ImplementationType>();
		}

		public void Register<TInterface, TImplementation>() where TImplementation : class
		{
			var interfaceType = typeof(TInterface);
			var implementationType = typeof(TImplementation);

			if (this.IsRegistered(interfaceType) || this.IsRegistered(implementationType))
			{
				throw new Exception("Type already Registered Exception");
			}

			this._registeredDependencyTypes.Add(interfaceType, new ImplementationType(implementationType));
			this._registeredTypes.Add(implementationType, this._registeredDependencyTypes[interfaceType]);
		}

		public void Register<TImplementation>() where TImplementation : class
		{
			var implementationType = typeof(TImplementation);

			if (this.IsRegistered(implementationType))
			{
				throw new Exception("Type already Registered Exception");
			}

			this._registeredTypes.Add(implementationType, new ImplementationType(implementationType));
		}

		public TRegistered GetInstance<TRegistered>()
		{
			return (TRegistered)this.GetInstance(typeof(TRegistered));
		}

		public object GetInstance(Type instanceType)
		{
			if (!this.IsRegistered(instanceType))
			{
				throw new Exception("TypeNotRegistered Exception");
			}

			ImplementationType implType = this.GetRegisteredTypeFromType(instanceType);
			object[] parameterInstances = GetParameterInstancesFromType(implType);

			return Activator.CreateInstance(instanceType, parameterInstances);
		}

		private object[] GetParameterInstancesFromType(ImplementationType implType)
		{
			Type[] parameters = this.GetParametersFromType(implType);

			object[] parameterInstances = new object[parameters.Length];

			for (int i = 0; i < parameters.Length; i++)
			{
				parameterInstances[i] = this.GetInstance(parameters[i]);
			}

			return parameterInstances;
		}

		private Type[] GetParametersFromType(ImplementationType implType)
		{
			Type[] parameters = new Type[implType.Parameters.Count];
			for (int i = 0; i < parameters.Length; i++)
			{
				var parameterType = implType.Parameters[i].ParameterType;
				if (this.IsRegistered(parameterType))
				{
					parameters[i] = parameterType;
				}
			}

			return parameters;
		}

		private ImplementationType GetRegisteredTypeFromType(Type instanceType)
		{
			ImplementationType implType = null;

			if (this._registeredDependencyTypes.ContainsKey(instanceType))
			{
				implType = this._registeredDependencyTypes[instanceType];
			}
			else if (this._registeredTypes.ContainsKey(instanceType))
			{
				implType = this._registeredTypes[instanceType];
			}

			return implType;
		}

		public bool IsRegistered(Type type)
		{
			if (this._registeredDependencyTypes.ContainsKey(type))
			{
				return true;
			}
			if (this._registeredTypes.ContainsKey(type))
			{
				return true;
			}
			return false;
		}

		public bool IsRegistered<T>()
		{
			return this.IsRegistered(typeof(T));
		}
	}
}

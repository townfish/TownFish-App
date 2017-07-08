// Copyright © Apptelic Ltd., 2011-16
// Licensed to TownFish Ltd. for use in internal projects only.
// Not for re-sale or re-distribution in source form; all other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;


namespace TownFish.App.ViewModels
{
	/// <summary>
	/// Provides base View Model functionality and automatic property change notification.
	/// </summary>
	public abstract class ViewModelBase: INotifyPropertyChanged, IDisposable
	{
		#region Construction

#if DEBUG
		/// <summary>
		/// Creates a new <see cref="ViewModelBase"/>.
		/// </summary>
		public ViewModelBase()
		{
			Debug.WriteLine ("ViewModelBase.ctor: {0}", (object) GetType().Name);
		}
#endif

		/// <summary>
		/// Releases managed and unmanaged resources.
		/// <param name="disposing">True to release managed resources</param>
		/// </summary>
		protected virtual void Dispose (bool disposing)
		{
			if (disposing)
			{
				// release managed resources
				// ...
			}

			// dispose of unmanaged resources
			// ...
		}

		/// <summary>
		/// Finalises this instance.
		/// </summary>
		~ViewModelBase()
		{
			Dispose (false);

			Debug.WriteLine ("ViewModelBase.dtor: {0}", (object) GetType().Name);
		}

		#region IDisposable Members

		/// <summary>
		/// Releases managed resources.
		/// </summary>
		public void Dispose()
		{
			System.Diagnostics.Debug.WriteLine ("ViewModelBase.Dispose: {0}", (object) GetType().Name);

			Dispose (true);

			// no need to call this now we're disposed
			GC.SuppressFinalize (this);
		}

		#endregion

		#endregion Construction

		#region Methods

		/// <summary>
		/// Gets the named value from the property value bag which was previously set by
		/// <see cref="Set{T} (T, string)"/>, calling default generator if value doesn't exist and
		/// storing default value for later use (i.e. default generator is only ever called once).
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="makeDefault">Optional default value generator</param>
		/// <param name="name">Property name whose value to get (don't specify; compiler-provided)</param>
		/// <returns></returns>
		public T Get<T> (Func<T> makeDefault = null, [CallerMemberName] string name = null)
		{
			return Get (name, GetBag<T>(), makeDefault);
		}

		/// <summary>
		/// Sets the value of the named property in the property value bag and raises
		/// a property changed event if the new value is different from the old value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">New value</param>
		/// <param name="name">Property name whose value to set (don't specify; compiler-provided)</param>
		/// <returns>True if a new value was set or false if value is unchanged</returns>
		/// <remarks>
		/// <note type="note">
		/// Storing property values in the property value bag, which is a <c>Dictionary{string, T}</c>,
		/// offers poorer performance than using your own backing fields and using
		/// <see cref="Set{T} (ref T, T, string)" /> field-based Set method instead;
		/// these Get and Set methods are provided for the interminably lazy.
		/// </note>
		/// </remarks>
		public bool Set<T> (T value, [CallerMemberName] string name = null)
		{
			var bag = GetBag<T>();
			if (Equals (Get (name, bag), value))
				return false;

			bag [name] = value;

#if DEBUG
			var v = value?.ToString() ?? "<null>";
			if (string.IsNullOrEmpty (v)) v = "\"\"";

			Debug.WriteLine ($"ViewModelBase.Set: Property {name} changed to {v}");
#endif

			OnPropertyChanged (name);

			return true;
		}

		/// <summary>
		/// Sets the value of the given field and raises a property changed event if the
		/// new value is different from the old value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="field"></param>
		/// <param name="value"></param>
		/// <param name="name">Property name whose value to set (don't specify; compiler-provided)</param>
		/// <returns>True if a new value was set or false if value is unchanged</returns>
		public bool Set<T> (ref T field, T value, [CallerMemberName] string name = null)
		{
			if (Equals (field, value))
				return false;

			field = value;

			OnPropertyChanged (name);

			return true;
		}

		/// <summary>
		/// Raises a property changed event for the given property name.
		/// </summary>
		/// <param name="name"></param>
		public void OnPropertyChanged ([CallerMemberName] string name = null)
		{
			if (!mPcea.TryGetValue (name, out PropertyChangedEventArgs pcea))
				mPcea [name] = pcea = new PropertyChangedEventArgs (name);

			PropertyChanged?.Invoke (this, pcea);

			IsDirty = true; // if something's changed, I'm dirty
		}

		/// <summary>
		/// Gets the name of the property referenced by the given expression.
		/// </summary>
		/// <remarks>
		/// Called with an expression, e.g.:
		/// <code>OnPropertyChanged (() => MyProperty);</code>
		/// </remarks>
		/// <typeparam name="T"></typeparam>
		/// <param name="propExp"></param>
		/// <returns></returns>
		public virtual string PropertyName<T> (Expression<Func<T>> propExp)
		{
			var memberExp = propExp.Body as MemberExpression;
			if (memberExp == null)
				throw new ArgumentException ("Expression body must be a member expression");

			return memberExp.Member.Name;
		}

		/// <summary>
		/// Enables observation of the property referenced by the given expression.
		/// </summary>
		/// <remarks>
		/// <note type="note">
		/// This method can be used to start observing a property in an <c>INotifyPropertyChanged</c>
		/// class which is either in the same instance as the caller or in a different class/instance
		/// altogether.
		/// </note>
		/// <para>
		/// In the latter case, the callback will be called through a
		/// <see cref="T:System.WeakReference{Action{T}}" /> in order to avoid memory leaks (in the case where
		/// the target instance is longer-lived than the caller), so make sure if you're observing a
		/// property on a different instance that the callback reference provided is strongly-referenced
		/// in the caller (i.e. stored in a field rather than a method-local var) otherwise the observer
		/// will be surprised by its own demise at some random point in time and callbacks will
		/// mysteriously stop.
		/// </para>
		/// </remarks>
		/// <typeparam name="T"></typeparam>
		/// <param name="propExp">Expression referencing the property to be observed</param>
		/// <param name="callback">Callback to receive new property value when changed</param>
		public virtual void ObserveProperty<T> (Expression<Func<T>> propExp, Action<T> callback)
		{
			var memberExp = propExp.Body as MemberExpression;
			if (memberExp == null)
				throw new ArgumentException ("Expression body must be a member expression");

			var propInfo = memberExp.Member as PropertyInfo;
			if (propInfo == null)
				throw new ArgumentException ("Member must be a property");

			var name = propInfo.Name;

			// if callback is in a different instance, use weak ref in case I'm longer-lived than target
			var weakCallback = callback.Target == this ? null : new WeakReference<Action<T>> (callback);
			var strongCallback = callback.Target == this ? callback : null;

			PropertyChangedEventHandler localHandler = null; // '= null' prevents unassigned error below

			// this lambda captures both weakCallback and localHandler, so neither will be GC'd until the
			// weakCallback target is no longer valid and PropertyChanged is unhooked for this observer

			localHandler = (s, e) =>
				{
					if (e.PropertyName == name)
					{
						Action<T> targetHandler;

						if (weakCallback != null)
							weakCallback.TryGetTarget (out targetHandler);
						else
							targetHandler = strongCallback;

						if (targetHandler != null)
							targetHandler ((T) propInfo.GetValue (this));
						else
							PropertyChanged -= localHandler; // target has gone away, so I'm not needed
					}
				};

			PropertyChanged += localHandler;
		}

		/// <summary>
		/// Raises a strongly-typed property changed event for the given property.
		/// </summary>
		/// <remarks>
		/// Called with an expression, e.g.:
		/// <code>OnPropertyChanged (() => MyProperty);</code>
		/// </remarks>
		/// <typeparam name="T"></typeparam>
		/// <param name="propExp"></param>
		protected virtual void OnPropertyChanged<T> (Expression<Func<T>> propExp)
		{
			OnPropertyChanged (PropertyName (propExp));
		}

		/// <summary>
		/// Determines whether this view model's data is valid.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Returns true by default; override to provide custom validation.
		/// </para>
		/// <note type="implement">
		/// If overriding, don't raise any UI in this method; just indicate
		/// whether the view model data changes are valid or not. Override
		/// <see cref="Save"/> in order to raise UI for invalid data.
		/// </note>
		/// </remarks>
		/// <returns>True by default</returns>
		public virtual bool IsValid() { return true; }

		/// <summary>
		/// Saves this view model's changes, if there are any and if they're valid.
		/// </summary>
		/// <remarks>
		/// <para>
		/// By default, this base implementation calls <see cref="IsValid"/> and, if valid,
		/// calls <see cref="SaveChanges"/>.
		/// </para>
		/// <note type="implement">
		/// Override to add UI notifying the user of invalid changes.
		/// </note>
		/// </remarks>
		/// <returns>True if save succeeds, or if there is nothing to save</returns>
		public virtual bool Save()
		{	
			if (!IsValid())
				return false;

			return SaveChanges();
		}

		/// <summary>
		/// Saves any changes made to this view model's data; called from <see cref="Save"/>
		/// after previously determining that there are changes and that those changes are valid.
		/// </summary>
		/// <remarks>
		/// Just returns true by default; override (no need to call base) to implement saving.
		/// </remarks>
		/// <returns>True if save succeeds</returns>
		public virtual bool SaveChanges() { return true; }

		/// <summary>
		/// Specifies that this view model's data has been initialised.
		/// </summary>
		/// <remarks>
		/// This must be called at the end of derived class's constructor in order to ensure the
		/// <see cref="IsDirty"/> flag starts off <c>false</c>, since setting initial property
		/// values in the constructor causes IsDirty to be set to <c>true</c>.
		/// </remarks>
		protected virtual void Initialised()
		{
			IsDirty = false;
		}

		/// <summary>
		/// Gets or creates a bag of T values.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		Dictionary<string, T> GetBag<T>()
		{
			// get bag of T values from value bags collection
			var t = typeof (T);

			if (!mValueBags.TryGetValue (t, out object obj))
				mValueBags [t] = obj = new Dictionary<string, T>();

			return obj as Dictionary<string, T>;
		}

		/// <summary>
		/// Gets the named value from the given bag, calling default generator if value doesn't exist
		/// and storing default value for later use (i.e. default generator is only ever called once).
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name">Property name whose value to get (don't specify; compiler-provided)</param>
		/// <param name="bag">Bag of T values</param>
		/// <param name="makeDefault">Optional default value generator</param>
		/// <returns></returns>
		T Get<T> (string name, Dictionary<string, T> bag, Func<T> makeDefault = null)
		{
			if (!bag.TryGetValue (name, out T value) && makeDefault != null)
				bag [name] = value = makeDefault();

			return value;
		}

		#endregion Methods

		#region Properties

		#region INotifyPropertyChanged Members

		/// <summary>
		/// Raised when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		/// <summary>
		/// Indicates whether this view model's data has changed.
		/// </summary>
		protected virtual bool IsDirty { get; set; }

		#endregion Properties

		#region Fields

		Dictionary<string, PropertyChangedEventArgs> mPcea =
				new Dictionary<string, PropertyChangedEventArgs>();

		Dictionary<Type, object> mValueBags = new Dictionary<Type, object>();

		#endregion Fields
	}
}

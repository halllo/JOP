using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace JustObjectsPrototype
{
	public abstract class ViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void Changed<T>(Expression<Func<T>> selectorExpression)
		{
			if (selectorExpression == null)
				throw new ArgumentNullException("selectorExpression");

			MemberExpression body = selectorExpression.Body as MemberExpression;
			if (body == null)
				throw new ArgumentException("The body must be a member expression");

			PropertyChanged(this, new PropertyChangedEventArgs(body.Member.Name));
		}
	}
}

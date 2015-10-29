using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace JustObjectsPrototype.UI
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

			Changed(body.Member.Name);
        }

		protected virtual void Changed(string memberName)
		{
			var pc = PropertyChanged;
			if (pc != null) PropertyChanged(this, new PropertyChangedEventArgs(memberName));
		}
	}
}

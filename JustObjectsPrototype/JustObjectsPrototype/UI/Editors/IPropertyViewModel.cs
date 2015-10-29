﻿using System;

namespace JustObjectsPrototype.UI.Editors
{
	public interface IPropertyViewModel
	{
		string Label { get; }
		object Value { get; }
		Type ValueType { get; }
		void Refresh();
	}
}

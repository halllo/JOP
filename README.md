JOP - JustObjectsPrototype
==========================
Create only your domain objects and get a prototype shell for free.

[![Build status](https://ci.appveyor.com/api/projects/status/ctsxu7rw3vx537op?svg=true)](https://ci.appveyor.com/project/halllo/jop)
[![NuGet downloads](https://img.shields.io/nuget/dt/JOP.svg)](https://www.nuget.org/packages/JOP)
[![Version](https://img.shields.io/nuget/v/JOP.svg)](https://www.nuget.org/packages/JOP)

How To Use
----------
Just create a new .NET 4.5 Console Application and add POCO classes for your prototype domain types, like in the example below.
```csharp
public class Invoice
{
   public Customer Receiver { get; set; }
   public decimal Amount { get; set; }

   public void Increase()
   {
      Amount += 1;
   }
}
public class Customer
{
   public string Name { get; set; }
}
```
Now that you modelled your prototype domain, "Install-Package JOP" and show the prototype UI.
```csharp
[STAThreadAttribute()]
public static void Main()
{
   JustObjectsPrototype.Show.With(new List<object> {}, new List<Type> { typeof(Invoice), typeof(Customer) });
}
```
This gets you a UI like in the screenshot below, with no predefined objects and your two types. You can then create and delete instances of your types and invoke their methods.
![Screenshot](https://raw.github.com/halllo/JOP/master/screenshot.png)

Happy prototyping!


Thanks
------
<div>Icons made by <a href="http://www.flaticon.com/authors/freepik" title="Freepik">Freepik</a>, <a href="http://www.flaticon.com/authors/google" title="Google">Google</a> from <a href="http://www.flaticon.com" title="Flaticon">www.flaticon.com</a>             is licensed by <a href="http://creativecommons.org/licenses/by/3.0/" title="Creative Commons BY 3.0">CC BY 3.0</a></div>
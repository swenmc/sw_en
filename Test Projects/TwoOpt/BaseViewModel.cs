using System;
using System.ComponentModel;
using System.Diagnostics;

namespace TwoOpt
{
   public abstract class BaseViewModel : INotifyPropertyChanged
   {
      #region INotifyPropertyChanged Members

      public event PropertyChangedEventHandler PropertyChanged;

      #endregion

      protected void OnPropertyChanged(string propertyName)
      {
         VerifyPropertyName(propertyName);
         var handler = PropertyChanged;
         handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }

      [Conditional("DEBUG")]
      private void VerifyPropertyName(string propertyName)
      {
         if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            throw new ArgumentNullException(GetType().Name + " does not contain property: " + propertyName);
      }
   }
}
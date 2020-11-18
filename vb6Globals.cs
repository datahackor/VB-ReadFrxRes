using System;

namespace ReadFrxRes1
{
     public class vb6Globals 
      { 
            private static Form1 _form1 = null;

            public static Form1 Form1
            {
              get {
                if(_form1 == null)
                {
                    _form1 = new Form1();
                }
                return _form1;
              }
              set {
                if(_form1 != null && !_form1.IsDisposed)
                {
                }
                else 
                    _form1 = value;
              }
            }

           public vb6Globals() {  }

      }
} 

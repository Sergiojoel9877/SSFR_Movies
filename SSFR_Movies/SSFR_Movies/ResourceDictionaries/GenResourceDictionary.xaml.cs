﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace SSFR_Movies.ResourceDictionaries
{
  
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GenResourceDictionary : ResourceDictionary
	{
      
		public GenResourceDictionary ()
		{
			InitializeComponent ();
		}
	}
}
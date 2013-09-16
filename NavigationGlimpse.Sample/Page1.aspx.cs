﻿using Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationGlimpse.Sample
{
    public partial class Page1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            StateContext.Bag.CustomData = new CustomData();
        }

        protected void Button_Click(object sender, EventArgs e)
        {
            StateContext.Bag.Number = 1;
        }
    }
}
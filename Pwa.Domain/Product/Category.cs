﻿using Pwa.Domain.Aggregates;
using Pwa.Domain.Base;
using System;
using System.Collections.Generic;

namespace Pwa.Domain.Product
{
    public class Category : BaseDetail, ICategoryAggregate
    {
        public string Title { get; private set; }

        public List<WebApplication> WebApplications { get; private set; }

        protected Category()
        {

        }

        public Category(string title)
        {
            Title = title;
            WebApplications = new List<WebApplication>();
            CreationDate = DateTime.Now;
            LastEditDate = DateTime.Now;
        }

        public void Edit(string title)
        {
            Title = title;
            LastEditDate = DateTime.Now;
        }
    }
}

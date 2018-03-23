﻿using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyShop.Services
{
    public class BasketService
    {
        IRepo<Product> productContext;
        IRepo<Basket> basketContext;

        public const string BasketSessionName = "eCommerceBasket";

        public BasketService(IRepo<Product> productContext, IRepo<Basket> basketContext)
        {
            this.basketContext = basketContext;
            this.productContext = productContext;
        }

        private Basket GetBasket(HttpContextBase httpContext, bool CreateIfNull)
        {
            HttpCookie cookie = httpContext.Request.Cookies.Get(BasketSessionName);

            Basket basket = new Basket();

            if(cookie != null)
            {
                string basketId = cookie.Value;
                if(!string.IsNullOrEmpty (basketId ))
                {
                    basket = basketContext.Find(basketId);
                }
                else
                {
                    if(CreateIfNull )
                    {
                        basket = CreateNewBasket(httpContext);
                    }
                }
            }
            else
            {
                if (CreateIfNull)
                {
                    basket = CreateNewBasket(httpContext);
                }
            }
            return basket;
        }
       private Basket CreateNewBasket(HttpContextBase httpContext)
        {
            Basket basket = new Basket();
            basketContext.Insert(basket);
            basketContext.Commit();

            HttpCookie cookie = new HttpCookie(BasketSessionName);
            cookie.Value = basket.Id;
            cookie.Expires = DateTime.Now.AddDays(1);
            httpContext.Response.Cookies.Add(cookie);

            return basket;
        }

        public void AddToBasket(HttpContextBase httpContext, string productId)
        {
            Basket basket = GetBasket(httpContext, true);
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.ProductId  == productId);
        }

    }
}

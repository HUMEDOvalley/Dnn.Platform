﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Web.Routing;

namespace Dnn.Mvc.Web.Routing
{
    public abstract class PreRouterBase : RouteBase
    {
        private RouteCollection _routeCollection;

        public RouteCollection RouteCollection
        {
            get
            {
                if (_routeCollection == null)
                {
                    // Get the default collection
                    _routeCollection = RouteTable.Routes;
                }
                return _routeCollection;
            }
            set { _routeCollection = value; }
        }

        protected virtual TResult ReRouteRequest<TResult>(Func<RouteBase, TResult> request) where TResult : class
        {
            // NOTE: No need to worry about RouteExistingFiles, as the RouteTable which called us should handle that
            // Lock the collection for reading
            //using (RouteCollection.GetReadLock())
            //{
            bool startRouting = false;
            foreach (RouteBase routeBase in RouteCollection)
            {
                // The first route we're going to call is the one after this route
                if (routeBase == this)
                {
                    startRouting = true;
                }
                else if (startRouting)
                {
                    TResult result = request(routeBase);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            //}
            return null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModestTree.Util;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject
{
    // Inherit from this and mark you class with [TestFixture] attribute to do some unit tests
    // For anything more complicated than this, such as tests involving interaction between
    // several classes, or if you want to use interfaces such as IInitializable or IDisposable,
    // then I recommend using ZenjectIntegrationTestFixture instead
    // See documentation for details
    public abstract class UnitTestFixture
    {
        DiContainer _container;

        protected DiContainer Container
        {
            get { return _container; }
            set { _container = value; }
        }
    }
}

using System;
using System.Linq;
using System.Reflection;

using ExactJson.Infra;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Infra
{
    public sealed class ReflectionUtilTests
    {
        private abstract class A
        {
        }
        
        private sealed class B : A
        {
        }
        
        private sealed class Demo
        {
            public int PropertyReadWrite { get; set; }
            public int PropertyReadOnly { get; } = 0;
            public int PropertyForMutation1 { get; private set; }
            public int PropertyForMutation2 { get; private set; }

            public void Mutate()
            {
                PropertyForMutation1 = 1;
            }

            public void Mutate(int value)
            {
                PropertyForMutation1 = value;
            }
            
            public void Mutate(int value1, int value2)
            {
                PropertyForMutation1 = value1;
                PropertyForMutation2 = value2;
            }
            
            public int Get()
            {
                return 1;
            }
            
            public int Get(int i)
            {
                return i;
            }
        }

        private sealed class DemoNoDefCtor
        {
            // ReSharper disable once UnusedParameter.Local
            public DemoNoDefCtor(int x) { }
        }

        #region CreateDefaultConstructor

        [Test]
        public void CreateDefaultConstructor_GivenInfoForDefCtor_ReturnsLambda()
        {
            var ctor = ReflectionUtil.CreateDefaultConstructor<Demo>(typeof(Demo).GetConstructor(Type.EmptyTypes));

            Assert.That(ctor, Is.Not.Null);
            Assert.That(ctor(), Is.Not.Null);
        }

        [Test]
        public void CreateDefaultConstructor_GivenNullInfo_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ReflectionUtil.CreateDefaultConstructor<Demo>((ConstructorInfo) null);
            });
        }

        [Test]
        public void CreateDefaultConstructor_GivenInfoWith1ArgCtor_ThrowsArgumentException()
        {
            var info = typeof(DemoNoDefCtor).GetConstructor(new[] { typeof(int) });

            Assert.Throws<ArgumentException>(() =>
                ReflectionUtil.CreateDefaultConstructor<Demo>(info));
        }

        [Test]
        public void CreateDefaultConstructor_GivenNullType_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ReflectionUtil.CreateDefaultConstructor<Demo>((Type) null);
            });
        }

        [Test]
        public void CreateDefaultConstructor_GivenTypeForDefCtor_ReturnsLambda()
        {
            var ctor = ReflectionUtil.CreateDefaultConstructor<Demo>(typeof(Demo));

            Assert.That(ctor, Is.Not.Null);
            Assert.That(ctor(), Is.Not.Null);
        }

        [Test]
        public void CreateDefaultConstructor_GivenTypeWithoutDefCtor_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                ReflectionUtil.CreateDefaultConstructor<Demo>(typeof(DemoNoDefCtor)));
        }

        #endregion

        #region CreatePropertyGetter

        [Test]
        public void CreatePropertyGetter_GivenNullInfo_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ReflectionUtil.CreatePropertyGetter<Demo, int>(null);
            });
        }

        [Test]
        public void CreatePropertyGetter_GivenInfoForPublicGetter_ReturnsLambda()
        {
            var demo = new Demo { PropertyReadWrite = 1 };

            var getter = ReflectionUtil.CreatePropertyGetter<Demo, int>(
                typeof(Demo).GetProperty(nameof(Demo.PropertyReadWrite)));

            Assert.That(getter, Is.Not.Null);
            Assert.That(getter(demo), Is.EqualTo(1));
        }

        #endregion

        #region CreatePropertySetter

        [Test]
        public void CreatePropertySetter_GivenNullInfo_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ReflectionUtil.CreatePropertySetter<Demo, int>(null);
            });
        }

        [Test]
        public void CreatePropertySetter_GivenInfoForReadOnlyProp_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                ReflectionUtil.CreatePropertySetter<Demo, int>(
                    typeof(Demo).GetProperty(nameof(Demo.PropertyReadOnly))));
        }

        [Test]
        public void CreatePropertySetter_GivenInfoForReadWriteProperty_ReturnsLambda()
        {
            var demo = new Demo();

            var setter = ReflectionUtil.CreatePropertySetter<Demo, int>(
                typeof(Demo).GetProperty(nameof(Demo.PropertyReadWrite)));

            Assert.That(setter, Is.Not.Null);

            setter(demo, 1);

            Assert.That(demo.PropertyReadWrite, Is.EqualTo(1));
        }

        #endregion

        #region CreateActionMethodInvoker

        [Test]
        public void CreateActionMethodInvoker0_GivenNullInfo_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ReflectionUtil.CreateActionMethodInvoker<object>(null);
            });
        }

        [Test]
        public void CreateActionMethodInvoker0_GivenInfoWith1Param_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                ReflectionUtil.CreateActionMethodInvoker<object>(
                    typeof(Demo).GetMethod(
                        nameof(Demo.Mutate), new[] { typeof(int) })));
        }

        [Test]
        public void CreateActionMethodInvoker0_GivenInfoWith0Param_ReturnsLambda()
        {
            var demo = new Demo();

            var invoker = ReflectionUtil.CreateActionMethodInvoker<object>(
                typeof(Demo).GetMethod(nameof(Demo.Mutate), Type.EmptyTypes));

            Assert.That(invoker, Is.Not.Null);

            invoker(demo);

            Assert.That(demo.PropertyForMutation1, Is.EqualTo(1));
        }

        [Test]
        public void CreateActionMethodInvoker1_GivenNullInfo_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ReflectionUtil.CreateActionMethodInvoker<object, object>(null);
            });
        }

        [Test]
        public void CreateActionMethodInvoker1_GivenInfoWith0Param_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                ReflectionUtil.CreateActionMethodInvoker<object, object>(
                    typeof(Demo).GetMethod(nameof(Demo.Mutate), Type.EmptyTypes)));
        }

        [Test]
        public void CreateActionMethodInvoker1_GivenInfoWith1Param_ReturnsLambda()
        {
            var demo = new Demo();

            var invoker = ReflectionUtil.CreateActionMethodInvoker<object, object>(
                typeof(Demo).GetMethod(nameof(Demo.Mutate), new[] { typeof(int) }));

            Assert.That(invoker, Is.Not.Null);

            invoker(demo, 1);

            Assert.That(demo.PropertyForMutation1, Is.EqualTo(1));
        }

        [Test]
        public void CreateActionMethodInvoker2_GivenNullInfo_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ReflectionUtil.CreateActionMethodInvoker<object, object, object>(null);
            });
        }

        [Test]
        public void CreateActionMethodInvoker2_GivenInfoWith0Param_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                ReflectionUtil.CreateActionMethodInvoker<object, object, object>(
                    typeof(Demo).GetMethod(nameof(Demo.Mutate), Type.EmptyTypes)));
        }

        [Test]
        public void CreateActionMethodInvoker2_GivenInfoWith2Param_ReturnsLambda()
        {
            var demo = new Demo();

            var invoker = ReflectionUtil.CreateActionMethodInvoker<object, object, object>(
                typeof(Demo).GetMethod(nameof(Demo.Mutate), new[] { typeof(int), typeof(int) }));

            Assert.That(invoker, Is.Not.Null);

            invoker(demo, 1, 2);

            Assert.That(demo.PropertyForMutation1, Is.EqualTo(1));
            Assert.That(demo.PropertyForMutation2, Is.EqualTo(2));
        }

        #endregion
        
        #region CreateFuncMethodInvoker

        [Test]
        public void CreateFuncMethodInvoker0_GivenNullInfo_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ReflectionUtil.CreateFuncMethodInvoker<object, int>(null);
            });
        }

        [Test]
        public void CreateFuncMethodInvoker0_GivenInfoWith1Param_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                ReflectionUtil.CreateFuncMethodInvoker<object, int>(
                    typeof(Demo).GetMethod(
                        nameof(Demo.Get), new[] { typeof(int) })));
        }

        [Test]
        public void CreateFuncMethodInvoker0_GivenInfoWith0Param_ReturnsLambda()
        {
            var demo = new Demo();

            var invoker = ReflectionUtil.CreateFuncMethodInvoker<object, int>(
                typeof(Demo).GetMethod(nameof(Demo.Get), Type.EmptyTypes));

            Assert.That(invoker, Is.Not.Null);
            Assert.That(invoker(demo), Is.EqualTo(1));
        }
        
        #endregion

        [TestCase(typeof(int), typeof(int))]
        [TestCase(typeof(int?), typeof(int))]
        [TestCase(typeof(object), typeof(object))]
        public void UnwrapNullable(Type type, Type unwrappedType)
        {
            Assert.That(ReflectionUtil.UnwrapNullable(type), Is.EqualTo(unwrappedType));
        }

        [Test]
        public void UnwrapNullable_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(()
                => ReflectionUtil.UnwrapNullable(null));
        }
        
        [TestCase(typeof(int), false)]
        [TestCase(typeof(int?), true)]
        [TestCase(typeof(object), false)]
        public void IsNullable(Type type, bool nullable)
        {
            Assert.That(ReflectionUtil.IsNullable(type), Is.EqualTo(nullable));
        }

        [Test]
        public void IsNullable_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(()
                => ReflectionUtil.IsNullable(null));
        }
        
        [TestCase(typeof(int), false)]
        [TestCase(typeof(int?), true)]
        [TestCase(typeof(object), true)]
        public void IsNullAssignable(Type type, bool nullAssignable)
        {
            Assert.That(ReflectionUtil.IsNullAssignable(type), Is.EqualTo(nullAssignable));
        }

        [Test]
        public void IsNullAssignable_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(()
                => ReflectionUtil.IsNullAssignable(null));
        }

        [Test]
        public void GetBaseTypes_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => {
                var _ = ReflectionUtil.GetBaseTypes(null).ToArray();
            });
        }
        
        [Test]
        public void GetBaseTypes()
        {
            Assert.That(typeof(B).GetBaseTypes(), Is.EquivalentTo(new[] {
                typeof(A),
                typeof(object)
            }));
        }
    }
}
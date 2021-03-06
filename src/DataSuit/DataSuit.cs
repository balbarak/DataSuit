using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DataSuit.Infrastructures;
using DataSuit.Interfaces;
using DataSuit.Providers;
using DataSuit.Reflection;
using System.Diagnostics;
using System;

using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("DataSuit.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace DataSuit
{

    public class Suit
    {
        private readonly ISettings _settings;
        private readonly Mapper _mapper;
        private Dictionary<string, IDataProvider> PendingFieldsWithProviders;

        public Suit() : this(new Settings())
        {

        }
        public Suit(ISettings settings)
        {
            _settings = settings;
            _mapper = new Mapper(_settings);
        }

        /// <summary>
        /// It loads built-in data for this suit.
        /// </summary>
        public void Load()
        {
            Resources.Load(_settings);
        }

        /// <summary>
        /// It starts to build providers for common usage. E.g. Build().Range("age1,age2", 20, 48)
        /// </summary>
        /// <returns></returns>
        public IMapping Build()
        {
            SetFieldsWithProviders();

            var map = new Mapping();
            PendingFieldsWithProviders = map.GetFieldsWithProviders;
            return map;
        }

        /// <summary>
        /// It starts to build providers for given T. 
        /// </summary>
        /// <returns></returns>
        public IMapping<T> Build<T>() where T : class, new()
        {
            SetFieldsWithProviders();

            var map = new Mapping<T>();
            PendingFieldsWithProviders = map.GetFieldsWithProviders;
            return map;
        }

        private void SetFieldsWithProviders()
        {
            if (PendingFieldsWithProviders != null)
            {
                _settings.AddProvider(PendingFieldsWithProviders);
                PendingFieldsWithProviders = null;
            }
        }

        /// <summary>
        /// When you want to be ensure there is no pending providers.
        /// </summary>
        public void EnsureNoPendingProviders()
        {
            SetFieldsWithProviders();
        }

        /// <summary>
        /// It export given settings as json string
        /// </summary>
        /// <returns></returns>
        public string Export()
        {
            SetFieldsWithProviders();

            return _settings.Export();
        }
        /// <summary>
        /// It import given data into settings.
        /// </summary>
        /// <param name="data">It should be in json format. Generated from Export()</param>
        public void Import(string data)
        {
            _settings.Import(data);
        }

        /// <summary>
        /// It constructors a generator with given T.
        /// </summary>
        /// <returns></returns>
        public IGenerator<T> GeneratorOf<T>() where T : class, new()
        {
            return new Generator<T>(this);
        }

        /// <summary>
        /// It constructs PrimitiveGenerator from this suit.
        /// </summary>
        /// <returns></returns>
        public IPrimitiveGenerator GeneratorOfPrimitives()
        {
            return new PrimitiveGenerator(this);
        }

        internal virtual void Generate<T>(T item, ISessionManager manager) where T : class, new()
        {
            SetFieldsWithProviders();

            _mapper.Map(item, manager);
        }

        internal virtual T GeneratePrimitive<T>(string name, ISessionManager manager)
        {
            return _mapper.GetPrimitive<T>(name, manager);
        }
    }

    public class DataSuitRunner
    {
        /// <summary>
        /// It works with TestSetupAttribute. Before calling this method. You have to set [TestSetup(typeof(YourSetupClass))]
        /// </summary>
        /// <returns></returns>
        public static Suit GetSuit()
        {
            StackTrace stackTrace = new StackTrace();
            MethodBase method = stackTrace.GetFrame(1).GetMethod();
            var attr = method.GetCustomAttribute<TestSetupAttribute>();

            var inte = attr.Suit.GetInterface("ISetupSuit");

            if (inte == null)
                throw new Exception("The type of class should be inherited from ISetupSuit.");

            var instance = (ISetupSuit)Activator.CreateInstance(attr.Suit);

            return instance.Suit;
        }
    }
}
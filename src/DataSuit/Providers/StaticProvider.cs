﻿using DataSuit.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using DataSuit.Enums;
using DataSuit.Infrastructures;

namespace DataSuit.Providers
{
    public class StaticProvider<T> : IStaticProvider<T>
    {
        private T staticData;

        public T Current => staticData;

        public ProviderType Type => ProviderType.Static;

        object IDataProvider.Current => staticData;

        public T Value => staticData;
        public Type TType => typeof(T);
        public StaticProvider()
        {
            
        }
        public StaticProvider(T val)
        {
            staticData = val;
        }


        /// <summary>
        /// Empty method
        /// </summary>
        public void MoveNext(ISessionManager manager)
        {
            
        }

        public void SetData(T val)
        {
            staticData = val;
        }
    }
}

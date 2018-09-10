﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModestTree;

namespace Zenject.Tests.TestDestructionOrder
{
    public class FooDisposable1 : IDisposable
    {
        public void Dispose()
        {
            Debug.Log("Destroyed FooDisposable1");
        }
    }
}

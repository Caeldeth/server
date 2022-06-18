﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hybrasyl.Objects;

namespace Hybrasyl.Scripting;

public interface IScriptable
{
    public IWorldObject WorldObject { get; set; }
    public string Name { get; }
    public string Type { get; }
    public byte X { get; }
    public byte Y { get; }
}
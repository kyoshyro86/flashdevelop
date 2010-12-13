﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PluginCore.Bridge
{
    public interface IBridgeSettings
    {
        /// <summary>
        /// Enable delegation to host system
        /// </summary>
        bool Active { get; }

        /// <summary>
        /// Use Flash CS from the host system
        /// </summary>
        bool TargetRemoteIDE { get; }

        /// <summary>
        /// Use file explorer of the host system
        /// </summary>
        bool UseRemoteExplorer { get; }

        /// <summary>
        /// Hidden location for sharing files between host and guest system
        /// </summary>
        string SharedFolder { get; }

        /// <summary>
        /// List of file extensions which must always be executed by Windows
        /// </summary>
        string[] AlwaysOpenLocal { get; }
    }
}

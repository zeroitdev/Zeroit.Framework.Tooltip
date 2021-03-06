﻿// ***********************************************************************
// Assembly         : Zeroit.Framework.Tooltip
// Author           : ZEROIT
// Created          : 12-21-2018
//
// Last Modified By : ZEROIT
// Last Modified On : 12-21-2018
// ***********************************************************************
// <copyright file="PopupEventArgs.cs" company="Zeroit Dev Technologies">
//    This program is for creating Tooltip controls.
//    Copyright ©  2017  Zeroit Dev Technologies
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <https://www.gnu.org/licenses/>.
//
//    You can contact me at zeroitdevnet@gmail.com or zeroitdev@outlook.com
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;

namespace Zeroit.Framework.Tooltip
{
    /// <summary>
    /// Class PopupEventArgs.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class PopupEventArgs : EventArgs
    {
        /// <summary>
        /// The size
        /// </summary>
        private System.Drawing.Size _size;
        /// <summary>
        /// Initializes a new instance of the <see cref="PopupEventArgs"/> class.
        /// </summary>
        public PopupEventArgs() : base()
        {
        }
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public System.Drawing.Size Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
            }
        }
    }
}

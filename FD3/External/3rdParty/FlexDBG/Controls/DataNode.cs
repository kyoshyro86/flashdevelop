﻿/*
    Copyright (C) 2009  Robert Nelson

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using Aga.Controls.Tree;
using Flash.Tools.Debugger;

namespace FlexDbg.Controls
{
	public class DataNode : Node, IComparable<DataNode>
    {
        public override string Text
        {
            get
			{
				return base.Text;
			}
        }

        private Variable m_Value;
		private bool m_bEditing = false;

		public int CompareTo(DataNode otherNode)
		{
			String thisName = m_Value.getName();
			String otherName = otherNode.m_Value.getName();

			if (thisName == otherName)
			{
				return 0;
			}

			if (thisName[0] == '_')
			{
				thisName = thisName.Substring(1);
			}

			if (otherName[0] == '_')
			{
				otherName = otherName.Substring(1);
			}

			int result = thisName.CompareTo(otherName);

			if (result != 0)
			{
				return result;
			}

			return m_Value.getName()[0] == '_' ? 1 : -1;
		}

		public string Value
        {
            get
			{
				if (m_Value == null)
				{
					return string.Empty;
				}

				int type = m_Value.getValue().getType();

				if (type == VariableType.MOVIECLIP || type == VariableType.OBJECT)
				{
					return m_Value.getValue().getTypeName().Replace("::", ".").Replace("@", " (@") + ")";
				}
				else if (type == VariableType.NUMBER)
				{
					double number = (double)m_Value.getValue().ValueAsObject;

					if (!Double.IsNaN(number) && (double)(long)number == number)
					{
						if (!m_bEditing)
						{
							if (number < 0 && number >= Int32.MinValue)
							{
								return number.ToString() + " [0x" + ((Int32)number).ToString("x") + "]";
							}
							else if (number < 0 || number > 9)
							{
								return number.ToString() + " [0x" + ((Int64)number).ToString("x") + "]";
							}
						}
						else
						{
							return number.ToString();
						}
					}
				}
				else if (type == VariableType.BOOLEAN)
				{
					return m_Value.getValue().ValueAsString.ToLower();
				}
				else if (type == VariableType.STRING)
				{
					if (m_Value.getValue().ValueAsObject != null)
					{
						if (!m_bEditing)
						{
							return "\"" + m_Value.ToString() + "\"";
						}
						else
						{
							return m_Value.ToString();
						}
					}
				}
				else if (type == VariableType.NULL)
				{
					return "null";
				}
				else if (type == VariableType.FUNCTION)
				{
					return "<setter>";
				}

				return m_Value.ToString();
			}

			set
			{
				if (m_Value == null)
				{
					return;
				}

				int type = m_Value.getValue().getType();

				if (type == VariableType.NUMBER)
				{
					m_Value.setValue(type, value);
				}
				else if (type == VariableType.BOOLEAN)
				{
					m_Value.setValue(type, value.ToLower());
				}
				else if (type == VariableType.STRING)
				{
					m_Value.setValue(type, value);
				}
			}
        }

		public Variable Variable
		{
			get
			{
				return m_Value;
			}
		}

        public override bool IsLeaf
        {
            get
            {
				if (m_Value == null)
				{
					return false;
				}

				return m_Value.getValue().getType() != VariableType.MOVIECLIP && 
					   m_Value.getValue().getType() != VariableType.OBJECT;
            }
        }

		public bool IsEditing
		{
			get
			{
				return m_bEditing;
			}

			set
			{
				m_bEditing = value;
			}
		}

        public DataNode(Variable value)
            : base(value.getName())
        {
            m_Value = value;
        }

		public DataNode(string value)
			: base(value)
		{
			m_Value = null;
		}
	}
}

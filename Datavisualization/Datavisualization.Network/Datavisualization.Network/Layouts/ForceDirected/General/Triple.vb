'! 
'@file Triple.cs
'@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
'		<http://github.com/juhgiyo/epForceDirectedGraph.cs>
'@date August 08, 2013
'@brief Generic Triple Interface
'@version 1.0
'
'@section LICENSE
'
'The MIT License (MIT)
'
'Copyright (c) 2013 Woong Gyu La <juhgiyo@gmail.com>
'
'Permission is hereby granted, free of charge, to any person obtaining a copy
'of this software and associated documentation files (the "Software"), to deal
'in the Software without restriction, including without limitation the rights
'to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
'copies of the Software, and to permit persons to whom the Software is
'furnished to do so, subject to the following conditions:
'
'The above copyright notice and this permission notice shall be included in
'all copies or substantial portions of the Software.
'
'THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
'IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
'FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
'AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
'LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
'OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
'THE SOFTWARE.
'
'@section DESCRIPTION
'
'An Interface for the Generic Triple Class.
'
'

Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Public Class Triple(Of T, U, V)
	Public Sub New()
	End Sub

	Public Sub New(iFirst As T, iSecond As U, iThird As V)
		Me.first = iFirst
		Me.second = iSecond
		Me.third = iThird
	End Sub

	Public Property first() As T
		Get
			Return m_first
		End Get
		Set
			m_first = Value
		End Set
	End Property
	Private m_first As T
	Public Property second() As U
		Get
			Return m_second
		End Get
		Set
			m_second = Value
		End Set
	End Property
	Private m_second As U
	Public Property third() As V
		Get
			Return m_third
		End Get
		Set
			m_third = Value
		End Set
	End Property
	Private m_third As V
End Class
﻿Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Language

Namespace ComponentModel.Collection

    ''' <summary>
    ''' 对数据进行分组，通过标签数据的相似度
    ''' </summary>
    Public Module FuzzyGroup

        ''' <summary>
        ''' Grouping objects in a collection based on their <see cref="sIdEnumerable.Identifier"/> string Fuzzy equals to others'.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="source"></param>
        ''' <param name="cut">字符串相似度的阈值</param>
        ''' <returns></returns>
        <Extension>
        Public Function FuzzyGroups(Of T As sIdEnumerable)(
                        source As IEnumerable(Of T),
               Optional cut As Double = 0.6,
               Optional parallel As Boolean = False) As GroupResult(Of T, String)()

            Return source.FuzzyGroups(Function(x) x.Identifier, cut, parallel).ToArray
        End Function

        ''' <summary>
        ''' Grouping objects in a collection based on their unique key string Fuzzy equals to others'.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="source"></param>
        ''' <param name="getKey">The unique key provider</param>
        ''' <param name="cut">字符串相似度的阈值</param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function FuzzyGroups(Of T)(
                                 source As IEnumerable(Of T),
                                 getKey As Func(Of T, String),
                        Optional cut As Double = 0.6,
                        Optional parallel As Boolean = False) As IEnumerable(Of GroupResult(Of T, String))

            Dim tmp As New List(Of __groupHelper(Of T))
            Dim list As List(Of __groupHelper(Of T)) =
                LinqAPI.MakeList(Of __groupHelper(Of T)) <= From x As T
                                                            In source
                                                            Let s_key As String = getKey(x)
                                                            Select New __groupHelper(Of T) With {
                                                                .cut = cut,
                                                                .key = s_key,
                                                                .keyASC = s_key.ToArray(AddressOf Asc),
                                                                .x = x
                                                            }
            Dim out As GroupResult(Of T, String)

            If parallel Then
                Call "Fuzzy grouping running in parallel mode...".__DEBUG_ECHO
            End If

            Do While list.Count > 0
                Dim ref As __groupHelper(Of T) = list(Scan0)

                Call tmp.Clear()
                Call tmp.Add(list(Scan0))   ' 重置缓存
                Call list.RemoveAt(Scan0)   ' 写入Group的参考数据

                If parallel Then
                    tmp += LQuerySchedule.LQuery(list, Function(x) x, Function(x) ref.Equals(x:=x))
                Else
                    For Each x As __groupHelper(Of T) In list
                        If ref.Equals(x:=x) Then
                            Call tmp.Add(x)
                        End If
                    Next
                End If

                For Each x As __groupHelper(Of T) In tmp
                    Call list.Remove(x)
                Next

                out = New GroupResult(Of T, String) With {
                    .Group = tmp.ToArray(Function(x) x.x),
                    .Tag = ref.key
                }
                Yield out
            Loop
        End Function

        ''' <summary>
        ''' 分组操作的内部帮助类
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        Private Structure __groupHelper(Of T)

            ''' <summary>
            ''' Key for represent this object.
            ''' </summary>
            Public key As String
            ''' <summary>
            ''' Target element object in the grouping 
            ''' </summary>
            Public x As T
            Public cut As Double
            ''' <summary>
            ''' Key cache
            ''' </summary>
            Public keyASC As Integer()

            Public Overrides Function ToString() As String
                Return Me.GetJson
            End Function

            ''' <summary>
            ''' 判断Key是否模糊相等
            ''' </summary>
            ''' <param name="x"></param>
            ''' <returns></returns>
            Public Overloads Function Equals(x As __groupHelper(Of T)) As Boolean
                Dim edits As DistResult =
                    ComputeDistance(keyASC, x.keyASC, Function(a, b) a = b,
                                    AddressOf Chr)

                If edits Is Nothing Then
                    Return False
                Else
                    Return edits.MatchSimilarity >= cut
                End If
            End Function
        End Structure
    End Module
End Namespace
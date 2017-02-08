﻿' Instat-R
' Copyright (C) 2015
'
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License k
' along with this program.  If not, see <http://www.gnu.org/licenses/>.

Imports instat
Imports instat.Translations
Public Class dlgPermuteColumn
    Private frmMain As New frmMain
    Private clsSetSampleFunc, clsSetSeedFunc, clsOverallFunction As New RFunction
    Private bFirstLoad As Boolean = True
    Private bReset As Boolean = True

    Private Sub dlgPermuteRows_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        autoTranslate(Me)
        If bFirstLoad Then
            InitialiseDialog()
            bFirstLoad = False
        End If
        If bReset Then
            SetDefaults()
        End If
        SetRCodeForControls(bReset)
        bReset = False
        TestOkEnabled()
    End Sub

    Public Sub SetRCodeForControls(bReset As Boolean)
        ucrReceiverPermuteRows.SetRCode(clsSetSampleFunc, bReset)
        ucrChkSetSeed.SetRCode(clsSetSeedFunc, bReset)
        ucrNudSetSeed.SetRCode(clsOverallFunction, bReset)
    End Sub

    Private Sub SetDefaults()
        Dim clsDefaultFunction, clsDefaultSample, clsDefaltSetSeed As New RFunction

        ucrPermuteRowsSelector.Reset()
        clsDefaultSample.SetRCommand("sample")
        clsDefaultSample.AddParameter("replace", "FALSE")
        clsDefaultSample.AddParameter("size", ucrPermuteRowsSelector.ucrAvailableDataFrames.iDataFrameLength)

        clsDefaltSetSeed.SetRCommand("set.seed")
        clsDefaltSetSeed.AddParameter("seed", 5)

        clsDefaultFunction.SetRCommand("replicate")

        clsSetSampleFunc = clsDefaultSample.Clone
        clsSetSeedFunc = clsDefaltSetSeed.Clone

        clsDefaultFunction.SetRCommand("replicate")
        clsDefaultFunction.AddParameter("n", 1)
        clsDefaultFunction.AddParameter("expr", clsRFunctionParameter:=clsSetSampleFunc)
        clsOverallFunction = clsDefaultFunction.Clone
        ucrBase.clsRsyntax.SetBaseRFunction(clsDefaultFunction)
        clsDefaultFunction.SetAssignTo(strTemp:=ucrSavePermute.GetText, strTempDataframe:=ucrPermuteRowsSelector.ucrAvailableDataFrames.cboAvailableDataFrames.Text, strTempColumn:=ucrSavePermute.GetText, bAssignToIsPrefix:=True)
    End Sub

    Private Sub ReopenDialog()

    End Sub

    Private Sub InitialiseDialog()

        ucrBase.iHelpTopicID = 66

        ucrReceiverPermuteRows.Selector = ucrPermuteRowsSelector
        ucrReceiverPermuteRows.SetMeAsReceiver()
        ucrReceiverPermuteRows.bUseFilteredData = False
        ucrReceiverPermuteRows.SetParameter(New RParameter("x", 0))
        ucrReceiverPermuteRows.SetParameterIsRFunction()
        ucrNudNumberofColumns.SetParameter(New RParameter("n", 1))
        ucrNudNumberofColumns.Minimum = 1
        ucrNudNumberofColumns.SetRDefault(1)
        ucrChkSetSeed.AddToLinkedControls(ucrNudSetSeed, {True}, bNewLinkedAddRemoveParameter:=True, bNewLinkedHideIfParameterMissing:=True, bNewLinkedUpdateFunction:=True)
        ucrChkSetSeed.SetText("Set Seed")
        ucrNudSetSeed.SetParameter(New RParameter("seed", 0))
        ucrNudSetSeed.Minimum = 1
        ucrNudSetSeed.SetRDefault(5)

        ucrChkSetSeed.AddFunctionNamesCondition("set.seed", True)
        ucrSavePermute.SetName("permute")
        ucrSavePermute.SetSaveTypeAsColumn()
        ucrSavePermute.SetDataFrameSelector(ucrPermuteRowsSelector.ucrAvailableDataFrames)
        ucrSavePermute.SetLabelText("Save Permute:")
        ucrSavePermute.SetIsComboBox()
    End Sub

    Private Sub SetSeed()
        If ucrChkSetSeed.Checked Then
            frmMain.clsRLink.RunInternalScript(clsSetSeedFunc.ToScript)
            clsSetSeedFunc.SetRCommand("set.seed")
        End If
    End Sub

    Private Sub TestOkEnabled()
        If Not ucrReceiverPermuteRows.IsEmpty AndAlso ucrSavePermute.IsComplete AndAlso ucrNudNumberofColumns.GetText > 0 Then
            ucrBase.OKEnabled(True)
        Else
            ucrBase.OKEnabled(False)
        End If
    End Sub

    Private Sub ucrBase_ClickReset(sender As Object, e As EventArgs) Handles ucrBase.ClickReset
        SetDefaults()
        SetRCodeForControls(True)
        TestOkEnabled()
    End Sub

    Private Sub ucrReceiverPermuteRows_ControlContentsChanged(ucrChangedControl As ucrCore) Handles ucrReceiverPermuteRows.ControlContentsChanged, ucrSavePermute.ControlContentsChanged, ucrNudNumberofColumns.ControlContentsChanged
        TestOkEnabled()
    End Sub

    Private Sub ucrChkSetSeed_ControlValueChanged(ucrChangedControl As ucrCore) Handles ucrChkSetSeed.ControlValueChanged
        SetSeed()
    End Sub
End Class
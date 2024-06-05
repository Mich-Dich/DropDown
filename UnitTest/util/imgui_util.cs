using Xunit;
using ImGuiNET;
using Core.util;
using System.Numerics;
using System;

public class Imgui_UtilTests
{
    [Fact]
    public void BeginTable_CallsExpectedFunctions()
    {
        // Arrange
        string label = "TestTable";
        bool displayLabel = true;

        var io = ImGui.GetIO();
        io.Fonts.AddFontDefault();

        // Act
        Imgui_Util.Begin_Table(label, displayLabel);

        // Assert (Check if ImGui functions were called)
        Assert.True(ImGui.IsItemActive());
        Assert.Equal(label, ImGui.GetItemID().ToString());
    }

    [Fact]
    public void EndTable_EndsTheTable()
    {
        // Arrange (Simulate a table being started)
        ImGui.BeginTable("DummyTable", 2);

        // Act
        Imgui_Util.End_Table();

        // Assert
        Assert.False(ImGui.IsItemActive());
    }

    [Fact]
    public void AddTableRow_StringOverload_AddsRowWithText()
    {
        // Arrange
        ImGui.BeginTable("DummyTable", 2);
        string label = "Test Label";
        string value = "Test Value";

        // Act
        Imgui_Util.Add_Table_Row(label, value);

        // Assert (Indirectly, as we cannot directly inspect the table content)
        Assert.True(ImGui.IsItemActive());
    }

    [Fact]
    public void AddTableRow_FloatOverload_AddsRowWithDragFloat()
    {
        // Arrange
        ImGui.BeginTable("DummyTable", 2);
        string label = "Test Float";
        float value = 5.0f;

        // Act
        Imgui_Util.Add_Table_Row(label, ref value);

        // Assert (Indirectly)
        Assert.True(ImGui.IsItemActive());
    }

    [Fact]
    public void AddTableRow_IntOverload_AddsRowWithDragInt()
    {
        // Arrange
        ImGui.BeginTable("DummyTable", 2);
        string label = "Test Int";
        int value = 10;

        // Act
        Imgui_Util.Add_Table_Row(label, ref value);

        // Assert (Indirectly)
        Assert.True(ImGui.IsItemActive());
    }

    [Fact]
    public void AddTableRow_LambdaOverload_CallsTheAction()
    {
        // Arrange
        ImGui.BeginTable("DummyTable", 2);
        string label = "Test Action";
        bool actionCalled = false;
        Action action = () => actionCalled = true;

        // Act
        Imgui_Util.Add_Table_Row(label, action);

        // Assert
        Assert.True(ImGui.IsItemActive());
        Assert.True(actionCalled);
    }

    [Fact]
    public void AddTableSpacing_AddsSpacing()
    {
        // Arrange
        ImGui.BeginTable("DummyTable", 2);

        // Act
        Imgui_Util.Add_Table_Spacing(2); // Add two spacing rows

        // Assert (Indirectly)
        Assert.True(ImGui.IsItemActive()); 
    }

    [Fact]
    public void ShiftCursorPos_ShiftsCursorCorrectly()
    {
        // Arrange
        var initialPos = new Vector2(10, 15);
        ImGui.SetCursorPos(initialPos);

        // Act
        Imgui_Util.Shift_Cursor_Pos(5, -3);

        // Assert
        Assert.Equal(new Vector2(15, 12), ImGui.GetCursorPos());
    }
}

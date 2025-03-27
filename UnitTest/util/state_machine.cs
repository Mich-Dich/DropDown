namespace UnitTest
{
    using Xunit;
    using Core.util;
    using System;
    using System.Collections.Generic;

    public class StateMachineTest
    {
        private class MockState : I_state<object>
        {
            public Type execute(object aiController, float delta_time)
            {
                return typeof(MockState);
            }

            public bool exit(object aiController)
            {
                return true;
            }

            public bool enter(object aiController)
            {
                return true;
            }
        }

        [Fact]
        public void TestPreCreateStates()
        {
            var stateMachine = new state_machine<object>(new object());
            stateMachine.Pre_Create_States(new List<Type> { typeof(MockState) });
            Assert.True(stateMachine.allStates.ContainsKey("MockState"));
        }

        [Fact]
        public void TestSetStartupState()
        {
            var stateMachine = new state_machine<object>(new object());
            stateMachine.Set_Statup_State(typeof(MockState));
            Assert.Equal("MockState", stateMachine.currentState);
        }

        [Fact]
        public void TestUpdate()
        {
            var stateMachine = new state_machine<object>(new object());
            stateMachine.Set_Statup_State(typeof(MockState));
            stateMachine.Update(0.1f);
            Assert.Equal("MockState", stateMachine.currentState);
        }

    }
}
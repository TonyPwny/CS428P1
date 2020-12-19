// Anthony Tiongson (ast119)
// List of resources used:
//      
//      

#region License

// A simplistic Behavior Tree implementation in C#
// Copyright (C) 2010-2011 ApocDev apocdev@gmail.com
// 
// This file is part of TreeSharp
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

// TODO: THIS WAS A NEW FILE -- MODIFY THIS HEADER
#endregion

using System;
using System.Collections.Generic;

namespace TreeSharpPlus
{
    /// <summary>
    /// Loop Success will continue executing their child indefinitely unless that child reports
    /// success. If the child reports success, the proceed loop will cease.
    /// </summary>
    public class DecoratorLoopSuccess : Decorator
    {
        /// <summary>
        ///     The number of iterations to run (-1 is infinite)
        /// </summary>
        public int Iterations { get; set; }

        public DecoratorLoopSuccess(Node child)
            : base(child)
        {
            this.Iterations = -1;
        }

        public DecoratorLoopSuccess(int iterations, Node child)
            : base(child)
        {
            this.Iterations = iterations;
        }

        public override IEnumerable<RunStatus> Execute()
        {
            // Keep track of the running iterations
            int curIter = 0;

            while(true)
            {
                this.DecoratedChild.Start();

                RunStatus result;
                while ((result = this.TickNode(this.DecoratedChild)) == RunStatus.Running)
                    yield return RunStatus.Running;

                this.DecoratedChild.Stop();

                // If the child succeeds, break and report the success
                if (result == RunStatus.Success)
                {
                    yield return RunStatus.Success;
                    yield break;
                }

                // Increase the iteration count and see if we're done
                curIter++;
                if ((Iterations > 0) && (curIter >= Iterations))
                {
                    yield return RunStatus.Success;
                    yield break;
                }

                // Take one tick to prevent infinite loops
                yield return RunStatus.Running;
            }
        }
    }
}
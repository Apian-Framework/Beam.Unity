using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class Vector2Tests
    {
        [Test]
        public void CheckZero()
        {
            Vector2 z = Vector2.zero;
            Assert.That(z.x, Is.EqualTo(0)); 
            Assert.That(z.y, Is.EqualTo(0));
        }

        [Test]
        public void CheckDot()
        {
           Assert.That( 
                Vector2.Dot( new Vector2(3,0), new Vector2(0,1)),
                Is.EqualTo(0));
           Assert.That( 
                Vector2.Dot( new Vector2(1,0), new Vector2(1,1)),
                Is.EqualTo(1f));  
           Assert.That( 
                Vector2.Dot( new Vector2(1,0), new Vector2(1,-1)),
                Is.EqualTo(1f));    
           Assert.That( 
                Vector2.Dot( new Vector2(3,2), new Vector2(-4,5)),
                Is.EqualTo(-2f));                                                 
        }    


        [Test]
        public void CheckMagnitude()
        {
            Vector2 z = new Vector2(5,0);
            Assert.That(z.magnitude, Is.EqualTo(5)); 
            z = new Vector2(3,4);
            Assert.That(z.magnitude, Is.EqualTo(5));
            z = new Vector2(0,0);
            Assert.That(z.magnitude, Is.EqualTo(0));
        }        

        // Operators

        // instance methods
        [Test]
        public void CheckNormalize()
        {        
            Vector2 v = new Vector2(3,4);
            v.Normalize();
            Assert.That(v.magnitude, Is.EqualTo(1));     
            
            v = new Vector2(3,4);
            Vector2 n = v.normalized;
            Assert.That(n.magnitude, Is.EqualTo(1));

            v = new Vector2(0,0);   
            v.Normalize();
            Assert.That(v.magnitude, Is.EqualTo(0));            
        }

        // static methods
        [Test]      
        public void CheckSignedAngle()
        {        
            Assert.That( 
                Vector2.SignedAngle( new Vector2(0,0), new Vector2(0,0)),
                Is.EqualTo(0)); // Makes no sense, but should be zero 
            Assert.That( 
                Vector2.SignedAngle( new Vector2(1,0), new Vector2(1,0)),
                Is.EqualTo(0f));      
             Assert.That(                 
                 Vector2.SignedAngle( new Vector2(1,0), new Vector2(-1,-0)),
                 Is.EqualTo(180f));   
            Assert.That(                 
                Vector2.SignedAngle( new Vector2(1,0), new Vector2(1,1)),
                Is.EqualTo(45f));             
            Assert.That(                 
                Vector2.SignedAngle( new Vector2(1,1), new Vector2(1,0)),
                Is.EqualTo(-45f));   
            Assert.That(                 
                Vector2.SignedAngle( new Vector2(1,0), new Vector2(-3,3)),
                Is.EqualTo(135f));   
            Assert.That(                 
                Vector2.SignedAngle( new Vector2(1,0), new Vector2(-3,-3)),
                Is.EqualTo(-135f)); 
        }

    }
}

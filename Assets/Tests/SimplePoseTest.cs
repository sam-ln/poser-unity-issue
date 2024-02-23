using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using NUnit.Framework;
using Pose;
using UnityEngine;

public class SimplePoseTest
{
    /// <summary>
    /// Test whether it is possible to get the method 'GetMethodDescriptor' of the class 'Dynamic Method'
    /// Fails, probably due to the fact that the method is not implemented in Mono.
    /// </summary>
    [Test]
    public void CanGetDynamicMethodGetMethodDescriptorUsingReflection()
    {
        var methods = typeof(DynamicMethod).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
        Debug.Log("Accessible (private) methods in the class DynamicMethod:");
        foreach (var method in methods)
        {
            Debug.Log(method.Name);
        }

        bool methodsContainGetMethodDescriptor = methods.ToList().Any(info => info.Name == "GetMethodDescriptor");
        Assert.True(methodsContainGetMethodDescriptor);
    }

    /// <summary>
    /// Tests whether Pose(r) can successfully shim a static method of a different class.
    /// </summary>
    [Test]
    public void CanShimStaticMethodUsingPoser()
    {
        Shim methodShim = Shim.Replace(() => SimpleStaticClass.SimpleStaticMethod()).With(
            () => (double)20);
        PoseContext.Isolate(() => { Assert.AreEqual(20, SimpleStaticClass.SimpleStaticMethod()); }, methodShim);
    }

    /// <summary>
    /// Tests whether methods that have the [SecurityCritical] attribute can be retrieved by Reflection.
    /// Passes, even though dotnet documentation suggests otherwise:
    /// https://learn.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/security-considerations-for-reflection#accessing-security-critical-members"
    /// </summary>
    [Test]
    public void CanReflectSecurityCriticalMethods()
    {
        MethodInfo methodInfo =
            typeof(SimpleStaticClass).GetMethod("SecurityCriticalMethod", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(methodInfo);
        // Check if the method is marked with [SecurityCritical]
        bool isSecurityCritical = Attribute.IsDefined(methodInfo, typeof(SecurityCriticalAttribute));
        Assert.True(isSecurityCritical);
    }
}
using System;

namespace DefaultEngine.Editor.Api.DependencyInjection;

[AttributeUsage(AttributeTargets.Class)]
public sealed class RegisterAttribute : Attribute;

[AttributeUsage(AttributeTargets.Class)]
public sealed class RegisterAttribute<T> : Attribute;

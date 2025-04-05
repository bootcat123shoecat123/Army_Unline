using Godot;
using System;
using System.Collections.Generic;

public static class Sprite2DExtensions
{
    // 大小动画相关
    private static Dictionary<Sprite2D, Tween> scaleTweens = new Dictionary<Sprite2D, Tween>();
    private static Dictionary<Sprite2D, Tween> textureTweens = new Dictionary<Sprite2D, Tween>();
    private static Dictionary<Sprite2D, ShaderMaterial> shaderMaterials = new Dictionary<Sprite2D, ShaderMaterial>();

    // 初始化着色器
    private static void InitializeShader(Sprite2D sprite)
    {
        if (!shaderMaterials.ContainsKey(sprite))
        {
            var shaderMaterial = new ShaderMaterial();
            shaderMaterial.Shader = GD.Load<Shader>("res://Shaders/sprite_transition.gdshader");
            sprite.Material = shaderMaterial;
            shaderMaterials[sprite] = shaderMaterial;
        }
    }

    // 大小动画扩展方法
    public static void AnimateScale(this Sprite2D sprite, Vector2 target, float duration = 0.5f)
    {
        if (scaleTweens.ContainsKey(sprite))
        {
            scaleTweens[sprite].Kill();
        }

        var tween = sprite.CreateTween();
        tween.TweenProperty(sprite, "scale", target, duration)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.Out);
        
        tween.TweenCallback(Callable.From(() => {
            scaleTweens.Remove(sprite);
        }));

        scaleTweens[sprite] = tween;
    }

    // 图片过渡扩展方法
    public static void TransitionTexture(this Sprite2D sprite, Texture2D newTexture, float duration = 0.5f)
    {
        if (textureTweens.ContainsKey(sprite))
        {
            textureTweens[sprite].Kill();
        }

        InitializeShader(sprite);
        var shaderMaterial = shaderMaterials[sprite];

        // 设置着色器参数
        shaderMaterial.SetShaderParameter("old_texture", sprite.Texture);
        shaderMaterial.SetShaderParameter("new_texture", newTexture);
        shaderMaterial.SetShaderParameter("transition_progress", 0.0f);

        var tween = sprite.CreateTween();
        tween.TweenProperty(shaderMaterial, "shader_parameter/transition_progress", 1.0f, duration)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.Out);

        tween.TweenCallback(Callable.From(() => {
            sprite.Texture = newTexture;
            shaderMaterial.SetShaderParameter("transition_progress", 0.0f);
            textureTweens.Remove(sprite);
        }));

        textureTweens[sprite] = tween;
    }

    // 淡入淡出扩展方法
    public static void Fade(this Sprite2D sprite, float targetAlpha, float duration = 0.5f)
    {
        if (textureTweens.ContainsKey(sprite))
        {
            textureTweens[sprite].Kill();
        }

        var tween = sprite.CreateTween();
        tween.TweenProperty(sprite, "modulate:a", targetAlpha, duration)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.Out);

        tween.TweenCallback(Callable.From(() => {
            textureTweens.Remove(sprite);
        }));

        textureTweens[sprite] = tween;
    }

    // 停止所有动画扩展方法
    public static void StopAllAnimations(this Sprite2D sprite)
    {
        if (scaleTweens.ContainsKey(sprite))
        {
            scaleTweens[sprite].Kill();
            scaleTweens.Remove(sprite);
        }

        if (textureTweens.ContainsKey(sprite))
        {
            textureTweens[sprite].Kill();
            textureTweens.Remove(sprite);
        }
    }

    // 清理资源扩展方法
    public static void Cleanup(this Sprite2D sprite)
    {
        StopAllAnimations(sprite);
        if (shaderMaterials.ContainsKey(sprite))
        {
            shaderMaterials.Remove(sprite);
        }
    }
} 
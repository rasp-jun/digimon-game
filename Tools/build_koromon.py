import bpy, math, os, sys
from mathutils import Vector

out_path = sys.argv[sys.argv.index("--") + 1]
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)

def material(name, color, metallic=0.0, roughness=0.42):
    mat = bpy.data.materials.new(name)
    mat.diffuse_color = (*color, 1)
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes.get('Principled BSDF')
    bsdf.inputs['Base Color'].default_value = (*color, 1)
    bsdf.inputs['Roughness'].default_value = roughness
    bsdf.inputs['Metallic'].default_value = metallic
    return mat

pink = material('Koromon Pink', (1.0, 0.12, 0.20), 0, .34)
dark = material('Mouth', (.16, .008, .018), 0, .5)
white = material('Eye White', (.98, .98, .95), 0, .2)
brown = material('Warm Iris', (.18, .055, .025), 0, .22)

def uv(name, loc, scale, mat, segments=48, rings=32):
    bpy.ops.mesh.primitive_uv_sphere_add(segments=segments, ring_count=rings, location=loc)
    obj=bpy.context.object; obj.name=name; obj.scale=scale; bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    obj.data.materials.append(mat)
    for poly in obj.data.polygons: poly.use_smooth=True
    return obj

body=uv('Koromon_Body',(0,0,1.0),(1.12,.91,.94),pink)
ear_l=uv('Ear.L',(-.88,0,1.48),(.34,.36,.88),pink)
ear_l.rotation_euler=(0,-.22,-.67)
ear_r=uv('Ear.R',(.88,0,1.48),(.34,.36,.88),pink)
ear_r.rotation_euler=(0,.22,.67)

# Merge the body and ear roots into one rounded, sculpt-like continuous skin.
for obj in (body,ear_l,ear_r): obj.select_set(True)
bpy.context.view_layer.objects.active=body
bpy.ops.object.join()
body.name='Koromon_Skin'
body.data.remesh_voxel_size=.055
bpy.context.view_layer.objects.active=body
bpy.ops.object.voxel_remesh()
body.data.materials.clear(); body.data.materials.append(pink)
for poly in body.data.polygons: poly.use_smooth=True

for side in (-1,1):
    eye=uv('Eye.L' if side<0 else 'Eye.R',(side*.39,-.78,1.20),(.25,.105,.31),white,36,24)
    iris=uv('Iris.L' if side<0 else 'Iris.R',(side*.39,-.875,1.18),(.105,.045,.17),brown,32,20)
    shine=uv('Shine',(side*.35,-.915,1.25),(.035,.018,.052),white,20,12)

# Open smiling mouth, gum ridge and individually modeled baby teeth.
mouth=uv('Mouth',(0,-.84,.77),(.48,.075,.25),dark,40,24)
for side in (-1,1):
    bpy.ops.mesh.primitive_cone_add(vertices=24, radius1=.075, radius2=0, depth=.23, location=(side*.20,-.925,.82), rotation=(math.pi,0,0))
    tooth=bpy.context.object; tooth.name='Tooth'; tooth.data.materials.append(white)

# Subtle underside makes the silhouette feel planted rather than spherical.
belly=uv('Soft_Belly',(0,.04,.43),(.72,.61,.22),pink,40,24)

bpy.ops.object.select_all(action='SELECT')
for obj in bpy.context.selected_objects:
    if obj.type=='MESH':
        bevel=obj.modifiers.new('Soft bevel','BEVEL'); bevel.width=.018; bevel.segments=2

os.makedirs(os.path.dirname(out_path), exist_ok=True)
bpy.ops.wm.save_as_mainfile(filepath=os.path.splitext(out_path)[0]+'.blend')
bpy.ops.export_scene.fbx(filepath=out_path, use_selection=True, add_leaf_bones=False, bake_anim=False, apply_scale_options='FBX_SCALE_ALL')
print('EXPORTED', out_path)

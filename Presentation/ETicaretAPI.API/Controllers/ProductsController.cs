using ETicaretAPI.Application.Features.Commands.Product.CreateProduct;
using ETicaretAPI.Application.Features.Commands.Product.RemoveProduct;
using ETicaretAPI.Application.Features.Commands.Product.UpdateProduct;
using ETicaretAPI.Application.Features.Commands.ProductImageFile.ChangeShowcaseImage;
using ETicaretAPI.Application.Features.Commands.ProductImageFile.DeleteProductImage;
using ETicaretAPI.Application.Features.Commands.ProductImageFile.UploadProductImage;
using ETicaretAPI.Application.Features.Queries.Product.GetAllProduct;
using ETicaretAPI.Application.Features.Queries.Product.GetByIdProduct;
using ETicaretAPI.Application.Features.Queries.ProductImageFile.GetProductImages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;


namespace ETicaretAPI.API.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize(AuthenticationSchemes = "Admin")] // bu controllerdaki end-pointlerde yetki kontrolü yapacak : eğerki yetkisi yok ise 401 döndüreccektir.
// istek atabilmek için => sorgunun header kısmında => Authorization key'ine karşılık => users cotrollerıbndaki login endpoint'inden gelen token'ı alıyoruz =<  Bearer Tokenı yapştırıyoruz(Bearer yazısından sonra 1 boşluk var) 
public class ProductsController : ControllerBase
{

    readonly private IMediator _mediator;
    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAllProductQueryRequest getAllProductQueryRequest)
    {
        // throw new Exception("Laylaylom");  // sunucuya 500 hatası : sunucuya erişilemedi döndürür.,
        //return Unauthorized(); // yetkisiz erişim 401 döndürmesi için
        //paremtrede verdiğimiz requestin responseni bu şekilde karşılıyoruz. : mimari böyle
        GetAllProductQueryResponse response = await _mediator.Send(getAllProductQueryRequest);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] GetByIdProductQueryRequest getByIdProductQueryRequest)
    {
        GetByIdProductQueryResponse response= await _mediator.Send(getByIdProductQueryRequest);
        return Ok(response);
    }

    //dış dünyadan gelecek prodoct işlemlerini entity ile karşılamıyacam view model ile karşılayıp ona göre işleme devam edecem.
    [HttpPost]
	[Authorize(AuthenticationSchemes = "Admin")] // controller bazında değilde action bazında yetkilendirme veriyoruz.
	public async Task<IActionResult> Post(CreateProductCommandRequest createProductCommandRequest)
    {
        CreateProductCommandResponse response =  await _mediator.Send(createProductCommandRequest);
        return StatusCode((int)HttpStatusCode.Created);
    }

    [HttpPut]
	[Authorize(AuthenticationSchemes = "Admin")]
	public async Task<IActionResult> Put([FromBody] UpdateProductCommandRequest updateProductCommandRequest)
    {
        UpdateProductCommandResponse response = await _mediator.Send(updateProductCommandRequest);
        return Ok();
    }

    [HttpDelete("{id}")] //id diye bir paremetre gelecek diye belirtiyoruz.
	[Authorize(AuthenticationSchemes = "Admin")]
	public async Task<IActionResult> Delete([FromRoute] RemoveProductCommandRequest removeProductCommandRequest)
    {
        RemoveProductCommandResponse response = await _mediator.Send(removeProductCommandRequest);
        return Ok();
    }

    [HttpPost("[action]")] // birden fazla post methodu olduğu için action ismi ile ayırmamız gerekiyor : şart = bizde kendi adını veriyoruz action kısmına.
						   // ...com/api/products?id=123  => querystring // birden fazla da olabilir deger alma belirsiz
	[Authorize(AuthenticationSchemes = "Admin")]
	public async Task<IActionResult> Upload([FromQuery] UploadProductImageCommandRequest uploadProductImageCommandRequest)
    {
        uploadProductImageCommandRequest.Files = Request.Form.Files;
        UploadProductImageCommandResponse response = await _mediator.Send(uploadProductImageCommandRequest);
        return Ok();
    }

    [HttpGet("[action]/{id}")] // ..com/api/products/123  : root data => ne geleceği belli ise
	[Authorize(AuthenticationSchemes = "Admin")]
	public async Task<IActionResult> GetProductImages([FromRoute] GetProductImagesQueryRequest getProductImagesQueryRequest)
    {
        List<GetProductImagesQueryResponse> response = await _mediator.Send(getProductImagesQueryRequest);
        return Ok(response);
    }

    [HttpDelete("[action]/{id}")]
	[Authorize(AuthenticationSchemes = "Admin")]
	public async Task<IActionResult> DeleteProductImage([FromRoute] RemoveProductImageCommandRequest removeProductImageCommandRequest, [FromQuery] string imageId)
    {
        removeProductImageCommandRequest.imageId = imageId;
        RemoveProductImageCommandResponse removeProductImageCommandResponse = await _mediator.Send(removeProductImageCommandRequest);
        return Ok();
    }

    [HttpGet("[action]")]
	[Authorize(AuthenticationSchemes = "Admin")]
    public async Task<IActionResult> ChangeShowcaseImage([FromQuery] ChangeShowcaseImageCommandRequest changeShowcaseImageCommandRequest)
    {
        ChangeShowcaseImageCommandResponse response = await _mediator.Send(changeShowcaseImageCommandRequest);
        return Ok(response);

	}

}
